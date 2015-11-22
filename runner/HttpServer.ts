///<reference path="typings/tsd.d.ts" />

import * as http from 'http';

import {debug, info}  from './logger';
import {formatString} from './formatString';
import PortAllocator  from './PortAllocator';

export default class HttpServer {
  private port: number;

  start() {
    this.port = PortAllocator.getUnassignedPort();

    info('binding HTTP server to port {0}', this.port);
  }

  stop() {
    info('stopping HTTP server on port {0}', this.port);
  }
};

//
////Lets define a port we want to listen to
//const PORT=8080;
//
////We need a function which handles requests and send response
//function handleRequest(request, response){
//      response.end('It Works!! Path Hit: ' + request.url);
//}
//
////Create a server
//var server = http.createServer(handleRequest);
//
////Lets start our server
//server.listen(PORT, function(){
//      //Callback triggered when server is successfully listening. Hurray!
//          console.log("Server listening on: http://localhost:%s", PORT);
//});
