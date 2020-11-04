import { Component, OnInit } from '@angular/core';
import { HubConnection, HubConnectionState } from '@aspnet/signalr';
import { HubConnectionBuilder } from '@aspnet/signalr/dist/esm/HubConnectionBuilder';
import { ITreeOptions } from '@circlon/angular-tree-component';
import { Guid } from "guid-typescript";

@Component({
  selector: 'app-socket',
  templateUrl: './socket.component.html',
  styleUrls: ['./socket.component.scss']
})
export class SocketComponent implements OnInit {

  private _folderBrowseHubConnection: HubConnection;
  private _fileReadHubConnection: HubConnection;

  options: ITreeOptions = {
    getChildren: this.getChildren.bind(this),
    useCheckbox: true,
    actionMapping: {
      mouse: {
        click: (tree, node, $event) => {
          this.readFile(node.data);
        }
      },
    },
  };

  constructor() {
    this._folderBrowseHubConnection = this.createConnection('https://localhost:44306/fileBrowser');
    this._fileReadHubConnection = this.createConnection('https://localhost:44306/csvReader');
    this.startConnection(this._folderBrowseHubConnection);
    this.startConnection(this._fileReadHubConnection);
  }

  nodes: any[] = [];
  csvFileContent: any;

  async ngOnInit(): Promise<void> {
    this.nodes = await this.getFilesAndFolders();
  }

  getChildren(node: any) {
    return this.getFilesAndFolders(node.data.path);
  }

  async getFilesAndFolders(root?: string): Promise<any> {
    await this.waitForConnection(this._folderBrowseHubConnection);
    const data = await this.request<any>(this._folderBrowseHubConnection, "GetPaths", root)
    const tempNodes = [];
    for (const d of data) {
      tempNodes.push({
        name: d.name,
        hasChildren: d.type == 'Folder',
        path: d.path,
        type: d.type
      })
    }
    return tempNodes;

  }

  async readFile(data: any): Promise<void> {
    if (data.type == 'Folder') {
      return;
    }

    const csvData = await this.request<any>(this._fileReadHubConnection, "Read", data.path, 1)
    this.csvFileContent = csvData.rows;
  }


  //#region Supported Methods

  private request<T>(hubConnection: HubConnection, functionName: string, ...params: any[]): Promise<T> {
    return new Promise((resolve, rejected) => {
      var responseAt = Guid.create().toString();
      hubConnection.on(responseAt, (data) => {
        console.log(data);
        hubConnection.off(responseAt);
        resolve(data);
      });
      hubConnection.send(functionName, ...params, responseAt)
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

  private waitForConnection(hubConnection: HubConnection): Promise<void> {
    return new Promise((resolve, rejected) => {
      const interval = setInterval(() => {
        if (hubConnection.state === HubConnectionState.Connected) {
          clearInterval(interval);
          resolve();
        }
      }, 1000);
    });
  }

  //#endregion
}