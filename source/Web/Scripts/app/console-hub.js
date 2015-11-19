/// <reference path='../jquery-2.1.3.js' />
/// <reference path='../jquery.signalR-2.2.0.js' />
/// <reference path='console.js' />
function ConsoleHub() {
  this.setUp = function(parent, welcome) {
    $.connection.consoleHub.client.block = function(block) {
      new Console(parent, welcome, block.SessionId).block(block.Lines);
    };

    $.connection.consoleHub.client.terminate = function(sessionId) {
      new Console(parent, welcome, sessionId).terminate();
    };

    $.connection.hub.error(function(error) {
      $('#status').attr('class', 'error').text(error.message);
    });

    $.connection.hub.disconnected(function() {
      $.connection.hub.start();
    });

    $.connection.hub.stateChanged(function(change) {
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

    $.connection.hub.logging = true;
    $.connection.hub.start();
  };
}
