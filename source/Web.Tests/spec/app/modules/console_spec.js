/// <reference path='../../../../Web/Scripts/app/modules/console.js' />
/// <reference path='../../spec_helper.js' />

describe(Console.name, function() {
  beforeEach(function() {
    this.parent = $('<div>').attr('id', 'parent-container');
    this.welcome = $('<div>').attr('id', 'welcome-container');
    setFixtures(this.parent);
    setFixtures(this.welcome);

    var fakeTerminal = function(that) {
      var terminal = jasmine.createSpyObj('Terminal', ['open', 'write']);
      terminal.rowContainer = document.createElement('div');

      that.terminal = spyOn(window, 'Terminal').and.callFake(function() {
        return terminal;
      });
    };

    fakeTerminal(this);

    this.console = new Console(this.parent, this.welcome, 'id');
  });

  it('has a session id', function() {
    expect(this.console.sessionId).toEqual('id');
  });

  describe('text received', function() {
    describe('new session started', function() {
      beforeEach(function() {
        this.console.text({ StartOffset: 0, Text: 'line 1' });
      });

      it('hides welcome message', function() {
        expect(this.welcome).toBeHidden();
      });

      it('creates a new terminal', function() {
        expect(this.parent.find('section#session-id')).toExist();
      });

      it('sets terminal title', function() {
        expect(this.parent.find('section#session-id header')).toHaveText('id');
      });

      it('creates container for xterm', function() {
        expect(this.parent.find('section#session-id div.term')).toExist();
      });

      it('creates a xterm instance', function() {
        expect(this.terminal().open).toHaveBeenCalledWith(this.parent.find('section#session-id div.term')[0]);
      });

      it('writes lines', function() {
        expect(this.terminal().write).toHaveBeenCalledWith('line 1');
      });
    });

    describe('text for running session', function() {
      beforeEach(function() {
        this.console.text({ StartOffset: 0, EndOffset: 'line 1'.length, Text: 'line 1' });
        this.console.text({ StartOffset: 'line 1'.length, Text: 'line 2' });
      });

      it('uses existing xterm instance', function() {
        expect(this.terminal().open.calls.count()).toEqual(1);
      });

      it('writes lines', function() {
        expect(this.terminal().write.calls.allArgs()).toEqual([['line 1'], ['line 2']]);
      });
    });

    describe('new session started with another session running', function() {
      it('creates a new terminal', function() {
        this.console.text({ StartOffset: 0, Text: 'line 1' });

        var second = new Console(this.parent, this.welcome, 'id-2');
        second.text({ StartOffset: 0, Text: 'line 1' });

        expect(this.parent.children()).toHaveLength(2);
      });
    });

    describe('delayed text for new session', function() {
      it('starts session with delayed text', function() {
        this.console.text({ StartOffset: 42, Text: 'line 2' });

        expect(this.terminal().write).toHaveBeenCalledWith('line 2');
      });
    });

    describe('delayed text for running session', function() {
      beforeEach(function() {
        this.console.text({ StartOffset: 0, EndOffset: 'first'.length, Text: 'first' });
        this.console.text({ StartOffset: 'firstlate'.length, Text: 'early' });
      });

      it('marks warning for terminal', function() {
        expect(this.terminal().rowContainer).toHaveClass('delayed');
      });

      describe('delay resolved', function () {
        beforeEach(function() {
          this.console.text({ StartOffset: 'first'.length, EndOffset: 'firstlate'.length, Text: 'late' });
        });

        it('reorders text', function() {
          expect(this.terminal().write.calls.allArgs()).toEqual([['first'], ['late'], ['early']]);
        });

        it('removes warning', function() {
          expect(this.terminal().rowContainer).not.toHaveClass('delayed');
        });
      });
    });
  });

  describe('session terminated', function() {
    it('disables the terminal', function() {
      this.console.text({ StartOffset: 0, Text: 'line 1' });
      this.console.terminate();

      expect(this.parent.find('section#session-id')).toHaveClass('terminated');
    });
  });
});
