/// <reference path='../../../../Web/Scripts/app/modules/console-hub.js' />
/// <reference path='../../spec_helper.js' />

describe(ConsoleHub.name, function() {
  beforeEach(function() {
    var fakeConsole = function(that) {
      var console = jasmine.createSpyObj('Console', ['text', 'terminate']);

      that.console = spyOn(window, 'Console').and.callFake(function() {
        return console;
      });
    };

    var fakeConnection = function(that) {
      $.connection = {
        consoleHub: {
          client: {
            text: null,
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
      that.welcome = $('#welcome-container');

      new ConsoleHub().setUp(that.parent, that.welcome);
    };

    fakeConsole(this);
    fakeConnection(this);
    createConsoleHub(this);
  });

  describe('setup', function() {
    it('starts the hub connection', function() {
      expect($.connection.hub.start).toHaveBeenCalled();
    });
  });

  describe('running', function() {
    describe('text received', function() {
      beforeEach(function() {
        this.text = { SessionId: 'id', Offset: 0, Text: 'text' };

        $.connection.consoleHub.client.text(this.text);
      });

      it('creates a console for the session', function() {
        expect(this.console).toHaveBeenCalledWith(this.parent, this.welcome, this.text.SessionId);
      });

      it('sends text to the console', function() {
        expect(this.console().text).toHaveBeenCalledWith(this.text);
      });
    });

    describe('session terminated', function() {
      beforeEach(function() {
        this.sessionId = 'session ID';

        $.connection.consoleHub.client.terminate(this.sessionId);
      });

      it('creates a console for the session', function() {
        expect(this.console).toHaveBeenCalledWith(this.parent, this.welcome, this.sessionId);
      });

      it('terminates the console', function() {
        expect(this.console().terminate).toHaveBeenCalled();
      });
    });
  });
});