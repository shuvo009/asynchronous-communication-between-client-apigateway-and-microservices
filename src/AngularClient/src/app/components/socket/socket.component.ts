import { Component, OnInit } from '@angular/core';
import { HubConnection } from '@aspnet/signalr';
import { HubConnectionBuilder } from '@aspnet/signalr/dist/esm/HubConnectionBuilder';
import { Guid } from "guid-typescript";

@Component({
  selector: 'app-socket',
  templateUrl: './socket.component.html',
  styleUrls: ['./socket.component.scss']
})
export class SocketComponent implements OnInit {

  private _folderBrowseHubConnection: HubConnection;
  private _fileReadHubConnection: HubConnection;

  constructor() {
    this._folderBrowseHubConnection = this.createConnection('https://localhost:44306/fileBrowser');
    this._fileReadHubConnection = this.createConnection('https://localhost:44306/csvReader');
    this.startConnection(this._folderBrowseHubConnection);
    this.startConnection(this._fileReadHubConnection);
  }

  //#region Supported Methods

  private request<T>(hubConnection: HubConnection, functionName: string, params: string): Promise<T> {
    return new Promise((resolve, rejected) => {
      var responseAt = Guid.create().toString();
      hubConnection.on(responseAt, (data) => {
        hubConnection.off(responseAt);
        resolve(data);
      });
      hubConnection.send(functionName, params, responseAt)
    })
  }


  private createConnection(url: string): HubConnection {
    return new HubConnectionBuilder()
      .withUrl(url)
      .build();
  }

  private startConnection(hubConnection: HubConnection): void {
    hubConnection
      .start()
      .then(() => {
        console.log('Hub connection started');
      })
      .catch(err => {
        console.log('Error while establishing connection, retrying...');
      });
  }
  //#endregion



  ngOnInit(): void {
  }
}