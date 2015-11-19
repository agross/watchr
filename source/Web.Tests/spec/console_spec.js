/// <reference path='../../Web/Scripts/app/console.js' />
/// <reference path='spec_helper.js' />
describe(Console.name, function() {
  beforeEach(function() {
    this.parent = $('<div>').attr('id', 'parent-container');
    this.welcome = $('<div>').attr('id', 'welcome-container');
    setFixtures(this.parent);
    setFixtures(this.welcome);
  });

  beforeEach(function() {
    this.console = new Console(this.parent, this.welcome, 'id');
  });

  it('should have a session id', function() {
    expect(this.console.sessionId).toEqual('id');
  });

  describe('block received', function() {
    describe('new session started', function() {
      beforeEach(function() {
        spyOn($.fn, 'animate');

        this.console.block([{ Index: 0, Html: 'line 1' }]);
      });

      it('should hide welcome message', function() {
        expect(this.welcome).toBeHidden();
      });

      it('should create a new console', function () {
        expect(this.parent.find('section#session-id')).toExist();
      });

      it('should set title', function() {
        expect(this.parent.find('section#session-id header')).toHaveText('id');
      });

      it('should create space for lines', function() {
        expect(this.parent.find('section#session-id div')).toExist();
      });

      it('should set lines', function() {
        expect(this.parent.find('section#session-id div pre')).toHaveText('line 1');
      });

      it('should scroll to the bottom of the lines', function() {
        var height = this.parent.find('section#session-id div')[0].scrollHeight;
        expect($.fn.animate).toHaveBeenCalledWith({ scrollTop: height }, 200);
      });
    });

    describe('new lines for running session', function() {
      beforeEach(function() {
        this.console.block([{ Index: 0, Html: 'line 1' }]);
        this.console.block([{ Index: 1, Html: 'line 2' }]);
      });

      it('should append lines', function() {
        expect(this.parent.find('section#session-id div').children()).toHaveLength(2);
      });

      it('should set line text', function() {
        expect(this.parent.find('section#session-id div pre')).toHaveText('line 1line 2');
      });
    });

    describe('updated lines for running session', function() {
      beforeEach(function() {
        this.console.block([{ Index: 0, Html: 'line 1' }]);
        this.console.block([{ Index: 0, Html: 'line 2' }]);
      });

      it('should overwrite lines', function() {
        expect(this.parent.find('#session-id div').children()).toHaveLength(1);
      });

      it('should set line text', function() {
        expect(this.parent.find('section#session-id div pre')).toHaveText('line 2');
      });
    });

    describe('empty line', function() {
      beforeEach(function() {
        this.console.block([{ Index: 0, Html: '' }]);
      });

      it('should write non-breaking space', function() {
        expect(this.parent.find('section#session-id div pre')).toHaveHtml('&nbsp;');
      });
    });

    describe('line with HTML', function() {
      beforeEach(function() {
        this.console.block([{ Index: 0, Html: "<span class='red'>red text</span>" }]);
      });

      it('should keep tags', function() {
        expect(this.parent.find('section#session-id div pre')).toHaveText('red text');
      });
    });

    describe('new session started with another session running', function() {
      it('should create a new console', function() {
        this.console.block([]);

        var console2 = new Console(this.parent, this.welcome, 'id-2');
        console2.block([]);

        expect(this.parent.children()).toHaveLength(2);
      });
    });

    describe('delayed second block for running session', function() {
      it('should not create a new console', function() {
        this.console.block([]);

        var second = new Console(this.parent, this.welcome, this.console.sessionId);
        second.block([]);

        expect(this.parent.children()).toHaveLength(1);
      });
    });
  });

  describe('session terminated', function() {
    it('should disable the console', function() {
      this.console.block([]);
      this.console.terminate();

      expect(this.parent.find('section#session-id')).toHaveClass('terminated');
    });
  });
});
