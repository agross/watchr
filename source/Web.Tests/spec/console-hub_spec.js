/// <reference path='../../Web/Scripts/app/console-hub.js' />
/// <reference path='spec_helper.js' />
describe(ConsoleHub.name, function() {
  beforeEach(function() {
    var fakeConsole = function(that) {
      var console = jasmine.createSpyObj('Console', ['block', 'terminate']);

      that.console = spyOn(window, 'Console').and.callFake(function() {
        return console;
      });
    };

    var fakeConnection = function(that) {
      $.connection = {
        consoleHub: {
          client: {
            block: null,
            terminate: null
          }
        },
        hub: {
          start: jasmine.createSpy('start'),
          error: jasmine.createSpy('error'),
          disconnected: jasmine.createSpy('disconnected'),
          stateChanged: jasmine.createSpy('stateChanged')
        }
      };
    };

    var createConsoleHub = function(that) {
      that.parent = $('#parent-container');
      that.sessionId = 'id';
      that.lines = [];

      new ConsoleHub().setUp(that.parent);
    };

    fakeConsole(this);
    fakeConnection(this);
    createConsoleHub(this);
  });

  describe('setup', function() {
    it('should start the hub connection', function() {
      expect($.connection.hub.start).toHaveBeenCalled();
    });
  });

  describe('running', function() {
    describe('block received', function() {
      beforeEach(function() {
        $.connection.consoleHub.client.block({ SessionId: this.sessionId, Lines: this.lines });
      });

      it('should create a console for the session', function() {
        expect(this.console).toHaveBeenCalledWith(this.parent, this.sessionId);
      });

      it('should send blocks to the console', function() {
        expect(this.console().block).toHaveBeenCalledWith(this.lines);
      });
    });

    describe('session terminated', function() {
      beforeEach(function() {
        $.connection.consoleHub.client.terminate(this.sessionId);
      });

      it('should create a console for the session', function() {
        expect(this.console).toHaveBeenCalledWith(this.parent, this.sessionId);
      });

      it('should terminate the console', function() {
        expect(this.console().terminate).toHaveBeenCalled();
      });
    });
  });
});