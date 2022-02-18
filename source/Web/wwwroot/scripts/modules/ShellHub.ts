import { ShellView, ShellViewSettings } from './ShellView';
import { Block } from './BufferedTerminal';
import {
  HubConnection,
  HubConnectionBuilder,
  IRetryPolicy,
  RetryContext,
} from '@microsoft/signalr';

interface ShellHubSettings extends ShellViewSettings {
  url: string;
  group?: string;
  status: JQuery<HTMLElement>;
}

export interface TextReceived extends Block {
  sessionId?: string;
}

class AlwaysRetryPolicy implements IRetryPolicy {
  nextRetryDelayInMilliseconds(_retryContext: RetryContext): number {
    return 5000;
  }
}

export class ShellHub {
  private _settings: ShellHubSettings;
  private _connection: HubConnection;

  public async setUp(settings: ShellHubSettings) {
    this._settings = settings;

    await this.connect(this.connection);
  }

  private get connection(): HubConnection {
    if (this._connection) {
      return this._connection;
    }

    this._connection = new HubConnectionBuilder()
      .withUrl(this._settings.url)
      .withAutomaticReconnect(new AlwaysRetryPolicy())
      .build();

    this._connection.onreconnecting(() => {
      this._settings.status.attr('class', 'warning');
      this._settings.status.text('Reconnecting...');
    });

    this._connection.onreconnected(() => {
      this._settings.status.attr('class', 'success');
      this._settings.status.text('Online');
    });

    this._connection.onclose(() => {
      this._settings.status.attr('class', 'error');
      this._settings.status.text('Disconnected');
    });

    this._connection.on('text', async (text: TextReceived) => {
      const view = new ShellView(this._settings, text.sessionId);
      await view.textReceived(text);
    });

    this._connection.on('terminate', (sessionId: string) => {
      const view = new ShellView(this._settings, sessionId);
      view.terminate();
    });

    return this._connection;
  }

  private async connect(connection: HubConnection) {
    try {
      this._settings.status.attr('class', 'warning');
      this._settings.status.text('Connecting...');

      await connection.start();

      this._settings.status.attr('class', 'success');
      this._settings.status.text('Online');

      try {
        await connection.invoke('joinGroup', this._settings.group);
        console.log('Joined group with ' + this._settings.group);
      } catch (error) {
        console.log('Could not join group. Error: ' + error);
      }
    } catch (error) {
      this._settings.status.attr('class', 'error');
      this._settings.status.text('Disconnected');

      setTimeout(async () => await this.connect(connection), 5000);
    }
  }
}
