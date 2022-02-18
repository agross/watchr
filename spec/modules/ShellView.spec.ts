import { BufferedTerminal } from '../../source/Web/wwwroot/scripts/modules/BufferedTerminal';
import { ShellView } from '../../source/Web/wwwroot/scripts/modules/ShellView';
import { TextReceived } from '../../source/Web/wwwroot/scripts/modules/ShellHub';
import * as BufferedTerminalFunctions from '../../source/Web/wwwroot/scripts/modules/BufferedTerminal';
import { FitAddon } from 'xterm-addon-fit';
import * as FitAddonFunctions from 'xterm-addon-fit';

describe(ShellView.name, () => {
  let terminal: BufferedTerminal;
  let fitAddon: FitAddon;
  let view: ShellView;
  let parent: JQuery<HTMLElement>;
  let hideOnConnection: JQuery<HTMLElement>;
  let sessionId: string;

  beforeEach(() => {
    sessionId = 'session id';

    parent = $('<div>').attr('id', 'parent-container');
    hideOnConnection = $('<div>').attr('id', 'welcome-container');
    setFixtures(parent.html());
    setFixtures(hideOnConnection.html());

    terminal = jasmine.createSpyObj('BufferedTerminal', {
      loadAddon: undefined,
      loadWebfontAndOpen: Promise.resolve(terminal),
      writeBuffered: { buffering: false },
    });

    spyOn(BufferedTerminalFunctions, 'BufferedTerminal').and.returnValue(
      terminal,
    );

    fitAddon = jasmine.createSpyObj('FitAddon', ['fit']);

    spyOn(FitAddonFunctions, 'FitAddon').and.returnValue(fitAddon);

    view = new ShellView(
      { parent: parent, hideOnConnection: hideOnConnection },
      sessionId,
    );
  });

  describe('text received', () => {
    let event: TextReceived;

    beforeEach(async () => {
      event = {
        sessionId: sessionId,
        startOffset: 0,
        endOffset: 'line 1'.length,
        text: 'line 1',
      };

      await view.textReceived(event);
    });

    it('hides welcome message', () => {
      expect(hideOnConnection).toBeHidden();
    });

    it('creates container inside parent', () => {
      expect(view.container.parent()).toEqual(parent);
    });

    it('creates a container', () => {
      expect(view.container).toExist();
    });

    it('sets view title', () => {
      expect(view.container.find('header')).toHaveText(sessionId);
    });

    it('creates container for xterm', () => {
      expect(view.container.find('div.term')).toExist();
    });

    it('creates a xterm instance', () => {
      expect((terminal as any).loadWebfontAndOpen).toHaveBeenCalledWith(
        view.container.find('div.term')[0],
      );
    });

    it('writes buffered lines', () => {
      expect(terminal.writeBuffered).toHaveBeenCalledWith(event);
    });

    it('fits the terminal to the screen', () => {
      expect(fitAddon.fit).toHaveBeenCalled();
    });

    describe('terminal is not buffering', () => {
      it('does not mark container as delayed', () => {
        expect(view.container).not.toHaveClass('delayed');
      });
    });

    describe('terminal is buffering', () => {
      beforeEach(async () => {
        (terminal.writeBuffered as jasmine.Spy).and.returnValue({
          buffering: true,
        });

        await view.textReceived(event);
      });

      it('marks container as delayed', () => {
        expect(view.container).toHaveClass('delayed');
      });
    });

    describe('session terminated', () => {
      beforeEach(() => {
        view.terminate();
      });

      it('marks container as terminated', () => {
        expect(view.container).toHaveClass('terminated');
      });
    });
  });
});
