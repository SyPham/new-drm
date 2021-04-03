import { Component, OnInit, ViewChild } from '@angular/core';
import { GridComponent } from '@syncfusion/ej2-angular-grids';
import { AlertifyService } from 'src/app/_core/_service/alertify.service';
import { PermissionService } from 'src/app/_core/_service/permission.service';
import { RoleService } from 'src/app/_core/_service/role.service';

@Component({
  selector: 'app-role',
  templateUrl: './role.component.html',
  styleUrls: ['./role.component.css']
})
export class RoleComponent implements OnInit {
  role: any;
  data: any;
  editSettings = { showDeleteConfirmDialog: false, allowEditing: false, allowAdding: true, allowDeleting: false, mode: 'Normal' };
  toolbarOptions = ['Add',  'Search'];
  @ViewChild('grid') grid: GridComponent;
  pageSettings = { pageCount: 20, pageSizes: true, pageSize: 10 };
  fieldsRoleType: object = { text: 'name', value: 'name' };
  filterSettings = { type: 'Excel' };
  roleTypeData: object;
  roleTypeID: any;
  roleItem: string;
  roleID: any;
  constructor(
    private roleService: RoleService,
    private alertify: AlertifyService,
    private permissionService: PermissionService
  ) { }

  ngOnInit() {
    this.role = {
    };
    this.getAllRole();
  }
  // api
  getAllRole() {
    this.roleService.getAll().subscribe(res => {
      this.data = res;
    });
  }

  create() {
    this.roleService.create(this.role).subscribe(() => {
      this.alertify.success('Add Role Successfully');
      this.getAllRole();
      this.role = {
      };
    });
  }

  update() {
    this.roleService.update(this.role).subscribe(() => {
      this.alertify.success('Add Role Successfully');
      // this.modalReference.close() ;
      this.getAllRole();
      this.role = {
      };
    });
  }
  delete(id) {
    this.alertify.confirm('Delete role', 'Are you sure you want to delete this role "' + id + '" ?', () => {
      this.roleService.delete(id).subscribe(() => {
        this.getAllRole();
        this.alertify.success('The role has been deleted');
      }, error => {
        this.alertify.error('Failed to delete the role');
      });
    });
  }
  changeRole(args, item) {
    console.log(args, item);
    this.roleID = item.id;
  }
  // end api
  putPermissionByRoleId() {
    const request = {
      permissions: {
        // functionID: this.functionID,
        //   roleID: this.roleID,
        //   actionID: this.actionID,
      }
    };
    this.permissionService.putPermissionByRoleId(this.roleID, request).subscribe(() => {
      this.alertify.success('Successfully');
    });
  }
  // grid event
  toolbarClick(args): void {
    switch (args.item.text) {
      /* tslint:disable */
      case 'Excel Export':
        this.grid.excelExport();
        break;
      /* tslint:enable */
      default:
        break;
    }
  }
  actionBegin(args) {
    if (args.requestType === 'save') {
      if (args.role === 'add') {
        this.role.id = 0;
        this.role.name = args.data.name;
        this.role.code = args.data.code;
        this.create();
      }
      if (args.role === 'edit') {
        this.role.id = args.data.id;
        this.role.name = args.data.name;
        this.role.code = args.data.code;
        this.alertify.error('Can not update this role', true);
        // this.update();
      }
    }
    if (args.requestType === 'delete') {
      this.alertify.error('Can not delete this role', true);
      // this.delete(args.data[0].id);
    }
  }
  actionComplete(e: any): void {
    if (e.requestType === 'add') {
      (e.form.elements.namedItem('name') as HTMLInputElement).focus();
      (e.form.elements.namedItem('id') as HTMLInputElement).disabled = true;
    }
  }
  // end event
  NO(index) {
    return (this.grid.pageSettings.currentPage - 1) * this.pageSettings.pageSize + Number(index) + 1;
  }

}
