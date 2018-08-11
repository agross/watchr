/// <reference path='../../lib/jquery/jquery.js' />
/// <reference path='../../lib/signalr/jquery.signalR.min.js' />
/// <reference path='console.js' />

function ConsoleHub() {
  this.setUp = function(window, parent, welcome) {
    var connect = function(connection) {
      connection
        .start()
        .done(function() {
          hub.invoke('joinGroup', window.location.search)
            .done(function() {
              console.log('Joined group with ' + window.location.search);
            }).fail(function(error) {
              console.log('Could not join group. Error: ' + error);
            });
        });
    };

    var connection = function() {
      var connection = $.hubConnection();
      connection.logging = true;

      connection.error(function(error) {
        $('#status').attr('class', 'error').text(error.message);
      });

      connection.disconnected(function() {
        connect(connection);
      });

      connection.stateChanged(function (change) {
        if (change.newState === $.signalR.connectionState.connecting) {
          $('#status').attr('class', 'warning');
          $('#status').text('Connecting...');
        }

        if (change.newState === $.signalR.connectionState.reconnecting) {
          $('#status').attr('class', 'warning');
          $('#status').text('Reconnecting...');
        }

        if (change.newState === $.signalR.connectionState.connected) {
          $('#status').attr('class', 'success');
          $('#status').text('Online');
        }

        if (change.newState === $.signalR.connectionState.disconnected) {
          $('#status').attr('class', 'error');
          $('#status').text('Disconnected');
        }
      });

      return connection;
    }();

    var hub = function() {
      var hub = connection.createHubProxy('consoleHub');

      hub.on('text', function(text) {
        new Console(parent, welcome, text.SessionId).text(text);
      });

      hub.on('terminate', function(sessionId) {
        new Console(parent, welcome, sessionId).terminate();
      });

      return hub;
    }();

    connect(connection);
  };
}
