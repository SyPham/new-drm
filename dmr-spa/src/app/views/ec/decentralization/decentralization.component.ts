import { Component, OnInit, ViewChild } from '@angular/core';
import { AccountService } from 'src/app/_core/_service/account.service';
import { AlertifyService } from 'src/app/_core/_service/alertify.service';
import { EditService, ToolbarService, PageService, PageSettingsModel, ToolbarItems, GridComponent, QueryCellInfoEventArgs } from '@syncfusion/ej2-angular-grids';
import { RoleService } from 'src/app/_core/_service/role.service';
import { IRole, IUserRole } from 'src/app/_core/_model/role';
import { IUserCreate, IUserUpdate } from 'src/app/_core/_model/user';
import { UserService } from 'src/app/_core/_service/user.service';
import { environment } from 'src/environments/environment';
import { Tooltip } from '@syncfusion/ej2-angular-popups';
import { IBuilding } from 'src/app/_core/_model/building';
import { NgbModal, NgbModalRef } from '@ng-bootstrap/ng-bootstrap';
import { CheckBoxSelectionService } from '@syncfusion/ej2-angular-dropdowns';
const DISPATCHER = 6;
@Component({
  selector: 'app-decentralization',
  templateUrl: './decentralization.component.html',
  styleUrls: ['./decentralization.component.css'],
  providers: [ToolbarService, EditService, PageService, CheckBoxSelectionService]
})
export class DecentralizationComponent implements OnInit {
  userData: any;
  public mode: string;
  buildings: IBuilding[];
  buildingDatas: IBuilding[];
  fieldsBuilding: object = { text: 'name', value: 'id' };
  fieldsLine: object = { text: 'name', value: 'id' };
  fieldsRole: object = { text: 'name', value: 'name' };
  editSettings = { showDeleteConfirmDialog: false, allowEditing: true, allowAdding: true, allowDeleting: true, mode: 'Normal' };
  buildingUsers: [];
  user: any;
  password = '';
  userID: number;
  buildingID: number;
  modalReference: NgbModalRef;
  toolbarOptions = ['Search'];
  toolbar = ['Add', 'Edit', 'Delete', 'Update', 'Cancel', 'ExcelExport', 'Search'];
  passwordFake = `aRlG8BBHDYjrood3UqjzRl3FubHFI99nEPCahGtZl9jvkexwlJ`;
  pageSettings = { pageCount: 20, pageSizes: true, pageSize: 10 };
  @ViewChild('grid') public grid: GridComponent;
  roles: IRole[];
  roleID: any;
  userCreate: IUserCreate;
  userUpdate: IUserUpdate;
  setFocus: any;
  locale = localStorage.getItem('lang');
  lines: IBuilding[];
  public fields: object = { text: 'name', value: 'id' };
  lineList = [];
  lineRemovingList = [];
  buildingList = [];
  buildingRemovingList = [];
  constructor(
    private accountService: AccountService,
    private roleService: RoleService,
    public modalService: NgbModal,
    private userService: UserService,
    private alertify: AlertifyService,
  ) { }

  ngOnInit() {
    this.mode = 'CheckBox';
    this.roleID = 0;
    this.buildingID = 0;
    this.getRoles();
    this.getBuildings();
    this.getAllUserInfo();
  }
  // life cycle ejs-grid
  createdUsers() {
  }
  onDoubleClick(args: any): void {
    this.setFocus = args.column; // Get the column from Double click event
  }
  actionBegin(args) {
    if (args.requestType === 'save' && args.action === 'edit') {
    }
  }
  tooltip(args: QueryCellInfoEventArgs) {
    if (args.column.field !== 'ID' && args.column.field !== 'password' && args.column.field !== 'option') {
      const tooltip: Tooltip = new Tooltip({
        content: args.data[args.column.field] + ''
      }, args.cell as HTMLTableCellElement);
    }
  }
  toolbarClick(args) {
    switch (args.item.text) {
      case 'Excel Export':
        this.grid.excelExport({ hierarchyExportMode: 'All' });
        break;
      default:
        break;
    }
  }
  actionComplete(args) {
    // if (args.requestType === 'beginEdit' ) {
    //   if (this.setFocus.field !== 'role' && this.setFocus.field !== 'building') {
    //     args.form.elements.namedItem(this.setFocus.field).focus(); // Set focus to the Target element
    //   }
    // }
    // if (args.requestType === 'add') {
    //   args.form.elements.namedItem('employeeID').focus(); // Set focus to the Target element
    // }
  }
  dataBound() {
   // document.querySelectorAll('button[aria-label=Update] > span.e-tbar-btn-text')[0].innerHTML = 'Save';
  }
  // end life cycle ejs-grid

  // api
  onChangeBuilding(args, data) {
    this.userID = data.id;
    this.buildingID = args.itemData.id;
  }
  onChangeRole(args, data) {
    this.userID = data.id;
    this.roleID = args.itemData.id;
  }
  getBuildings() {
    this.accountService.getBuildings().subscribe((result: any) => {
      this.buildingDatas = result || [];
      const data = this.buildingDatas.filter((item: any) => item.level === 2);
      this.lines = this.buildingDatas.filter((item: any) => item.level === 3);
      this.buildings = this.buildingDatas.filter((item: any) => item.level === 2);
      this.buildingDatas = data;
    }, error => {
      this.alertify.error(error);
    });
  }
  getLineByUserID() {
    this.accountService.getLineByUserID(this.userID, this.buildingID).subscribe(result => {
      const lines = result.data;
      this.lines = lines;
    }, error => {
      this.alertify.error(error);
    });
  }
  getBuildingByUserID() {
    this.accountService.getBuildingByUserID(this.userID).subscribe(result => {
      const buildings = result.data;
      this.buildings = buildings;
    }, error => {
      this.alertify.error(error);
    });
  }
  getRoles() {
    this.roleService.getAll().subscribe(result => {
      this.roles = result;
    }, error => {
      this.alertify.error(error);
    });
  }
  mappingUserRole(userRole: IUserRole) {
    this.roleService.mappingUserRole(userRole).subscribe(result => {
      this.alertify.success('Successfully!');
      this.roleID = 0;
    }, error => {
      this.alertify.error(error);
    });
  }
  lock(data) {
    const obj: IUserRole = {
      userID: data.id,
      roleID: data.userRoleID,
      isLock: !data.isLock
    };
    this.lockAPI(obj);
  }
  mapBuildingUser(userid, buildingid) {
    if (userid !== undefined && buildingid !== undefined) {
      this.accountService.mapBuildingUser(userid, buildingid).subscribe((res: any) => {
        if (res.status) {
          this.alertify.success(res.message);
          this.getAllUserInfo();
          this.buildingID = 0;
        } else {
          this.alertify.success(res.message);
        }
      });
    }
  }

  mapUserRole(userID: number, roleID: number) {
    this.roleService.mapUserRole(userID, roleID).subscribe((res: any) => {
      if (res.status) {
        this.alertify.success(res.message);
        this.getAllUserInfo();
        this.roleID = 0;
      } else {
        this.alertify.success(res.message);
      }
    });
  }
  lockAPI(userRole: IUserRole) {
    this.roleService.lock(userRole).subscribe((res: any) => {
      if (res) {
        this.alertify.success('Success!');
        this.getAllUserInfo();
      } else {
        this.alertify.error('Failed!');
      }
    });
  }
  getAllUserInfo() {
    this.userService.getAllUserInfo().subscribe((users: any) => {
      this.userData = users.filter(item => item.userRoleID === DISPATCHER);
    });
  }
  delete(id) {
    this.accountService.deleteUser(id).subscribe(res => {
      this.alertify.success('The user has been deleted!');
      this.getAllUserInfo();
    });
  }
  create() {
    this.accountService.createUser(this.userCreate).subscribe((res: number) => {
      this.alertify.success('The user has been created!');
      if (res > 0) {
        if (res > 0) {
          this.mapBuildingUser(res, this.buildingID);
        }
        if (res > 0) {
          this.mapUserRole(res, this.roleID);
        }
        this.getAllUserInfo();
        this.password = '';
      }
    });
  }
  update() {
    this.accountService.updateUser(this.userUpdate).subscribe(res => {
      this.alertify.success('The user has been updated!');
      if (this.userID && this.buildingID) {
        this.mapBuildingUser(this.userID, this.buildingID);
      }
      if (this.userID > 0 && this.roleID > 0) {
        this.mapUserRole(this.userID, this.roleID);
      }
      this.getAllUserInfo();
      this.password = '';
    });
  }
  removing(args) {
    this.lineList = this.lineList.filter(item => item !== args.itemData.id);
    this.lineRemovingList.push(args.itemData.id);
    console.log('tag', this.lineRemovingList);
  }
  onSelect(args) {
    const data = args.itemData;
    this.lineList.push(data.id );
    console.log('tag', this.lineList);
  }
  removingBuilding(args) {
    this.buildingList = this.buildingList.filter(item => item !== args.itemData.id);
    this.buildingRemovingList.push(args.itemData.id);
    console.log('tag', this.buildingRemovingList);
  }
  onSelectBuilding(args) {
    const data = args.itemData;
    this.buildingList.push(data.id);
    console.log('tag', this.buildingList);
  }
  // end api
  NO(index) {
    return (this.grid.pageSettings.currentPage - 1) * this.pageSettings.pageSize + Number(index) + 1;
  }
  // checkboxChange(args, data) {
  //   console.log('checkboxChange', args);
  //   const checked = args.checked;
  //   if (checked === true) {
  //     this.accountService.mapLineUser(this.userID, data.id).subscribe(res => {
  //       if (res.status) {
  //         this.alertify.success(res.message);
  //         this.getLineByUserID();
  //       } else {
  //         this.alertify.warning(res.message);
  //       }
  //     }, err => this.alertify.error(err));
  //   } else {
  //     const obj = {
  //       userID: this.userID, buildingID: data.id
  //     };
  //     this.accountService.removeLineUser(obj).subscribe(res => {
  //         this.alertify.success('Thành công!');
  //         this.getLineByUserID();
  //     }, err => this.alertify.error(err));
  //   }
  // }
  checkboxChangeBuilding(args, data) {
    const checked = args.checked;
    if (checked === true) {
      const obj = {
        userID: this.userID, buildings: this.lineList
      };
      this.accountService.mapMultipleBuildingUser(obj).subscribe(res => {
          this.alertify.success('Thành công!');
          this.getBuildingByUserID();
      }, err => this.alertify.error(err));
    } else {
      const obj = {
        userID: this.userID, buildings: this.lineRemovingList
      };
      this.accountService.removeMultipleBuildingUser(obj).subscribe(res => {
        this.alertify.success('Thành công!');
        this.getLineByUserID();
      }, err => this.alertify.error(err));
    }
  }
  onCreatedLine(value) {
    // this.userID = value.id;
    // this.buildingID = this.buildingDatas.filter(item => item.name === value.building)[0].id;
    // this.getLineByUserID();
  }
  onCreatedBuilding(value) {
    // this.userID = value.id;
    // this.getBuildingByUserID();
  }
  showLineModal(name, value) {
    this.userID = value.id;
    this.buildingID = this.buildingDatas.filter(item => item.name  === value.building)[0].id;
    this.getLineByUserID();
    this.modalReference = this.modalService.open(name, { size: 'md' });
  }
  showBuildingModal(name, value) {
    this.userID = value.id;
    this.getBuildingByUserID();
    this.modalReference = this.modalService.open(name, { size: 'md' });
  }
}
