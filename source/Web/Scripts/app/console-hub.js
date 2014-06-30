/// <reference path='console.js' />
function ConsoleHub() {
  this.setUp = function(parent) {
    $.connection.consoleHub.client.block = function(block) {
      new Console(parent, block.SessionId).block(block.Lines);
    };

    $.connection.consoleHub.client.terminate = function(sessionId) {
      new Console(parent, sessionId).terminate();
    };

    $.connection.hub.start();
  };
}