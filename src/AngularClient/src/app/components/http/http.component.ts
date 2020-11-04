import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { ITreeOptions } from '@circlon/angular-tree-component';

@Component({
  selector: 'app-http',
  templateUrl: './http.component.html',
  styleUrls: ['./http.component.scss']
})
export class HttpComponent implements OnInit {

  constructor(private httpClient: HttpClient) { }
  nodes: any[] = [];
  csvFileContent: any;

  async ngOnInit(): Promise<void> {
    this.nodes = await this.getFilesAndFolders();
  }

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

  getChildren(node: any) {
    return this.getFilesAndFolders(node.data.path);
  }

  async getFilesAndFolders(root?: string): Promise<any> {
    let api = "https://localhost:44306/api/FileBrowser/GetPaths"
    if (root) {
      api = `${api}?root=${root}`;
    }
    const data = await this.get(api);
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
    const api = `https://localhost:44306/api/CsvReader/Read/1/?path=${data.path}`
    const csvData = await this.get(api);
    this.csvFileContent = csvData.rows;
  }


  get(api: string): Promise<any> {
    return new Promise((resolve, reject) => {
      this.httpClient.get<any>(api).subscribe((response) => {
        resolve(response);
      }, (error) => {
        reject(error);
      })
    });
  }

}
