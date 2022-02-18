import { ShellHub } from '../../source/Web/wwwroot/scripts/modules/ShellHub';
import * as ShellViewFunctions from '../../source/Web/wwwroot/scripts/modules/ShellView';
import { ShellView } from '../../source/Web/wwwroot/scripts/modules/ShellView';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';

describe(ShellHub.name, () => {
  let group: string;
  let connection: HubConnection;
  let parent;
  let hideOnConnection;
  let status;

  beforeEach(() => {
    connection = jasmine.createSpyObj('HubConnection', {
      onreconnected: undefined,
      onreconnecting: undefined,
      onclose: undefined,
      start: Promise.resolve(),
      on: undefined,
      invoke: Promise.resolve(),
    });

    spyOn(HubConnectionBuilder.prototype, 'build').and.returnValue(connection);

    parent = $('#parent-container');
    hideOnConnection = $('#welcome-container');
    status = $('#status');
    group = '?something';
  });

  describe('setUp', () => {
    beforeEach(async () => {
      await new ShellHub().setUp({
        parent: parent,
        hideOnConnection: hideOnConnection,
        status: status,
        url: '/something',
        group: group,
      });
    });

    it('handles reconnect attempts', () => {
      expect(connection.onreconnecting).toHaveBeenCalledWith(
        jasmine.any(Function),
      );
    });

    it('handles reconnects', () => {
      expect(connection.onreconnected).toHaveBeenCalledWith(
        jasmine.any(Function),
      );
    });

    it('handles disconnects', () => {
      expect(connection.onclose).toHaveBeenCalledWith(jasmine.any(Function));
    });

    it('handles hub "text" events', () => {
      expect(connection.on).toHaveBeenCalledWith('text', jasmine.any(Function));
    });

    it('handles hub "terminate" events', () => {
      expect(connection.on).toHaveBeenCalledWith(
        'terminate',
        jasmine.any(Function),
      );
    });

    it('starts the connection', () => {
      expect(connection.start).toHaveBeenCalled();
    });

    it('joins group with query string', () => {
      expect(connection.invoke).toHaveBeenCalledWith('joinGroup', group);
    });
  });

  describe('runtime', () => {
    const sessionId = 'session id';
    let view: ShellView;

    beforeEach(async () => {
      view = jasmine.createSpyObj('ShellView', ['textReceived', 'terminate']);

      spyOn(ShellViewFunctions, 'ShellView').and.returnValue(view);

      await new ShellHub().setUp({
        parent: parent,
        hideOnConnection: hideOnConnection,
        status: status,
        url: '/something',
      });
    });

    describe('text received', () => {
      let text = {
        sessionId: sessionId,
        startOffset: 0,
        endOffset: 'text'.length,
        text: 'text',
      };

      beforeEach(() => {
        const subscriber = (connection.on as jasmine.Spy).calls.argsFor(0)[1];
        subscriber(text);
      });

      it('sends text to the view', () => {
        expect(view.textReceived).toHaveBeenCalledWith(text);
      });
    });

    describe('session terminated', () => {
      beforeEach(() => {
        const subscriber = (connection.on as jasmine.Spy).calls.argsFor(1)[1];
        subscriber(sessionId);
      });

      it('terminates the view', () => {
        expect(view.terminate).toHaveBeenCalled();
      });
    });
  });
});
