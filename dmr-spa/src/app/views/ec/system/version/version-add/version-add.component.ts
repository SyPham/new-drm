import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { ChangeEvent } from '@ckeditor/ckeditor5-angular';
import ClassicEditor from '@haifahrul/ckeditor5-build-rich';
import { AlertifyService } from 'src/app/_core/_service/alertify.service';
import { VersionService } from 'src/app/_core/_service/version.service';
import HtmlEmbed from '@ckeditor/ckeditor5-html-embed/src/htmlembed';
@Component({
  selector: 'app-version-add',
  templateUrl: './version-add.component.html',
  styleUrls: ['./version-add.component.scss']
})
export class VersionAddComponent implements OnInit {
  editor = ClassicEditor;
  config = {
    toolbar: [

      'undo',
      'redo',
      '|',
      'heading',
      'fontFamily',
      'fontSize',
      '|',
      'bold',
      'italic',
      'underline',
      'fontColor',
      'fontBackgroundColor',
      'highlight',
      '|',
      'link',
      'CKFinder',
      'imageUpload',
      'mediaEmbed',
      '|',
      'alignment',
      'bulletedList',
      'numberedList',
      '|',
      'indent',
      'outdent',
      '|',
      'insertTable',
      'blockQuote',
      'specialCharacters',
      '|',
      'htmlEmbed',
    ],
    language: 'id',
    image: {
      toolbar: [
        'imageTextAlternative',
        'imageStyle:full',
        'imageStyle:side'
      ]
    },
  };
  version: {
    id: number,
    description: string,
    name: string
  };
  name: string;
  description: string;
  Id: any;
  isEdit: boolean;
  title: string;
  constructor(
    private versionService: VersionService,
    private alertify: AlertifyService,
    private router: Router,
    private route: ActivatedRoute,
  ) { }

  ngOnInit() {
    this.Id = +this.route.snapshot.params.id || 0;
    if (this.Id === 0) {
      this.isEdit = false;
      this.description = '';
      this.title = 'Add version';
    } else {
      this.isEdit = true;
      this.title = 'Edit version';
      this.getByID();
    }
    this.version = {
      id: this.Id,
      description: '',
      name: ''
    };
  }
  public onChange({ editor }: ChangeEvent) {
    const data = editor.getData();
    this.version.description = data;
    console.log(data);
  }
  create() {
    this.version = {
      id: 0,
      description: this.description,
      name: this.name
    };
    this.versionService.create(this.version).subscribe(() => {
      this.alertify.success('Add Version Successfully');
      this.back();
    });
  }
  reset() {
    this.description = '';
    this.name = '';
    this.version = {
      id: 0,
      description: this.description,
      name: this.name
    };
  }
  edit() {
    this.version = {
      id: this.Id,
      description: this.description,
      name: this.name
    };
    this.versionService.update(this.version).subscribe(() => {
      this.alertify.success('Update Version Successfully');
      this.back();
    });
  }
  getByID() {
    this.versionService.getById(this.Id).subscribe((item: any) => {
      this.version = item;
      this.description = this.version.description;
    });
  }
  save() {

    if (this.Id === 0) {
      this.create();
    } else {
      this.edit();
    }
  }
  back() {
    this.router.navigate(['/ec/system/version']);
  }
}
