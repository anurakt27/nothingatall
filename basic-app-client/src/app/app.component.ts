import { Component } from '@angular/core';
import { AbstractControl, FormArray, FormControl } from '@angular/forms';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  fileList = new FormArray([])
  emails: string[] = [];
  supportedFileTypes: string[] = ["application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "application/vnd.ms-excel"]

  constructor(private http: HttpClient) { }

  add(files: FileList): void {
    if(files){
      for(let i=0; i<files.length; i++)
      {
        if (this.fileList.controls.findIndex(x => x.value.name == files.item(i).name) == -1 
          && this.supportedFileTypes.includes(files.item(i).type))
        {
          this.fileList.controls.push(new FormControl(files.item(i)))
        }
      }
    }
  }

  remove(file: AbstractControl): void {
    const index = this.fileList.controls.indexOf(file);

    if (index >= 0) {
      this.fileList.controls.splice(index, 1);
    }
  }

  onSubmit(){
    var allPromises = [];
    this.fileList.controls.forEach(x => {
      var formData = new FormData();
      formData.append("file", x.value);
      allPromises.push(
        this.http.post('http://localhost:5000/api/Home', formData).toPromise()
        );
    })

    Promise.all(allPromises).then((values) => {
      this.fileList.clear();
      this.emails = [];
      values.forEach(x => {
        x.forEach(email => this.emails.push(email))
      })
    })
    .catch(err => console.log(err));
  }
}
