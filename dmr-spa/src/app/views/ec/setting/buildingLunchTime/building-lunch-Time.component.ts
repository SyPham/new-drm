import { Component, OnInit, ViewChild } from '@angular/core';
import { BuildingUserService } from 'src/app/_core/_service/building.user.service';
import { AlertifyService } from 'src/app/_core/_service/alertify.service';
import { IBuilding } from 'src/app/_core/_model/building';
import { NgbModal, NgbModalRef } from '@ng-bootstrap/ng-bootstrap';
import { BuildingLunchTimeService } from 'src/app/_core/_service/building.lunch.time.service';
import { PeriodMixing } from 'src/app/_core/_model/period.mixing';
import { GridComponent } from '@syncfusion/ej2-angular-grids';
import { PeriodDispatch } from 'src/app/_core/_model/period.dispatch';
import { LunchTimeService } from 'src/app/_core/_service/lunch.time.service';

@Component({
  selector: 'app-building-lunch-time',
  templateUrl: './building-lunch-Time.component.html',
  styleUrls: ['./building-lunch-Time.component.scss']
})
export class BuildingLunchTimeComponent implements OnInit {

  @ViewChild('grid') grid: GridComponent;

  toolbar: object;
  data: any;
  periods = [];
  periodsDispatch = [];
  buildingID: number;
  userData: any;
  buildingUserData: any;
  buildings: any;
  steps = {hour: 1, minute: 30 };
  modalReference: NgbModalRef;
  toolbarOptions = ['Add', 'Delete', 'Search'];
  fields: object = { text: 'content', value: 'content' };
  lunchTimeData = [];
  editSettings: { showDeleteConfirmDialog: boolean; allowEditing: boolean; allowAdding: boolean; allowDeleting: boolean; mode: string; };
  editSettingsPeriod: { showDeleteConfirmDialog: boolean;
  allowEditing: boolean; allowAdding: boolean; allowDeleting: boolean; mode: string; };
  filterSettings = { type: 'Excel' };
  startTime: Date;
  endTime: Date;

  startTimePeriodDispatch: Date;
  endTimePeriodDispatch: Date;
  lunchTimeID: number;
  periodMixing: PeriodMixing = {} as PeriodMixing;
  periodDispatch: PeriodDispatch = {} as PeriodDispatch;
  periodMixingID: number;
  building: IBuilding;
  constructor(
    private buildingLunchTimeService: BuildingLunchTimeService,
    private lunchTimeService: LunchTimeService,
    private alertify: AlertifyService,
    public modalService: NgbModal,
  ) { }

  ngOnInit() {
    this.editSettingsPeriod = { showDeleteConfirmDialog: false,
      allowEditing: true, allowAdding: true, allowDeleting: true, mode: 'Normal' };
    this.editSettings = { showDeleteConfirmDialog: false, allowEditing: true, allowAdding: true, allowDeleting: true, mode: 'Normal' };
    this.toolbar = ['Search'];
    this.getAllLunchTime();
  }
  created() {
    this.getBuildings();
  }
  rowSelected(args) {
    const data = args.data.entity || args.data;
    this.buildingID = Number(data.id);
  }
  onChangeStartTime(value: Date): void {
    this.startTime = value;

  }
  onChangeEndTime(value: Date): void {
    this.endTime = value;

  }
  onSelectLunchTime(args, data) {
    this.building = {} as IBuilding;
    this.building.id = data.id;
    this.building.lunchTimeID = args.itemData.id;
    this.buildingLunchTimeService.addLunchTimeBuilding(this.building).subscribe(() => {
      this.alertify.success('success!');
      this.getBuildings();
    }, err => {
      this.alertify.error(err);
    });
  }
  updatePeriod(item) {
    this.buildingLunchTimeService.updatePeriodMixing(item).subscribe((result: any) => {
      this.getPeriodMixingByBuildingID();
      this.alertify.success('success!');
    }, error => {
      this.alertify.error(error);
    });
  }
  getBuildings() {
    this.buildingLunchTimeService.getBuildings().subscribe((result: any) => {
      this.buildings = result || [];
      const data = this.buildings.filter((item: any) => item.level === 2);
      this.buildings = data;
    }, error => {
      this.alertify.error(error);
    });
  }
  getAllLunchTime() {
    this.lunchTimeService.getAllLunchTime().subscribe((result: any) => {
      this.lunchTimeData = result || [];
    }, error => {
      this.alertify.error(error);
    });
  }
  getPeriodMixingByBuildingID() {
    this.buildingLunchTimeService.getPeriodMixingByBuildingID(this.buildingID).subscribe((res: any) => {
      this.periods = res.map(item => {
        return {
          isOvertime: item.isOvertime,
          startTime: new Date(item.startTime),
          endTime: new Date(item.endTime),
          id: item.id
        };
      });
    });
  }
  showModal(name, value) {
    this.buildingID = +value.id;
    this.building = value as IBuilding;
    this.getPeriodMixingByBuildingID();
    this.modalReference = this.modalService.open(name, { size: 'xl' });
  }

  getPeriodDispatchByPeriodMixingID() {
    this.buildingLunchTimeService.getPeriodDispatchByPeriodMixingID(this.periodMixingID).subscribe((res: any) => {
      this.periodsDispatch = res.map(item => {
        return {
          isOvertime: item.isOvertime,
          startTime: new Date(item.startTime),
          endTime: new Date(item.endTime),
          id: item.id
        };
      });
    });
  }
  showPeriodDispatchModal(name, value) {
    this.periodMixing = value;
    this.periodMixingID = +value.id;
    this.getPeriodDispatchByPeriodMixingID();
    this.modalReference = this.modalService.open(name, { size: 'xl' });
  }
  actionBeginPeriodsGrid(args) {
    if (args.requestType === 'save') {
      if (args.action === 'add') {
        this.periodMixing.startTime = this.startTime;
        this.periodMixing.endTime = this.endTime;
        this.periodMixing.buildingID = this.buildingID;
        this.create();
      }
      if (args.action === 'edit') {
        this.periodMixing = args.data;
        this.periodMixing.startTime = this.startTime;
        this.periodMixing.endTime = this.endTime;
        this.periodMixing.buildingID = this.buildingID;
        this.update();
      }
    }
    if (args.requestType === 'delete') {
      this.delete(args.data[0].id);
    }
  }
  create() {
    this.buildingLunchTimeService.addPeriodMixing(this.periodMixing).subscribe(() => {
      this.alertify.success('Successfully');
      this.getPeriodMixingByBuildingID();
      this.periodMixing = {} as PeriodMixing;
    }, error => {
      this.alertify.error(error);
      this.getPeriodMixingByBuildingID();
    });
  }

  update() {
    this.buildingLunchTimeService.updatePeriodMixing(this.periodMixing).subscribe(() => {
      this.alertify.success('Successfully');
      this.getPeriodMixingByBuildingID();
      this.periodMixing = {} as PeriodMixing;
    }, error => {
      this.getPeriodMixingByBuildingID();
      this.alertify.error(error);
    });
  }
  delete(id) {
    this.alertify.confirm('Delete', 'Are you sure you want to delete this "' + id + '" ?', () => {
      this.buildingLunchTimeService.deletePeriodMixing(id).subscribe(() => {
        this.getPeriodMixingByBuildingID();
        this.alertify.success('Successfully');
      }, error => {
        this.alertify.error(error);
        this.getPeriodMixingByBuildingID();
      });
    });
  }
  changeOvertime(args, data) {
    this.periodMixing = data;
    this.periodMixing.buildingID = this.buildingID;
    this.periodMixing.isOvertime = args.checked as boolean;
    this.update();
  }
    changeOvertimePeriodDispatch(args, data) {
    this.periodDispatch = data;
    this.periodDispatch.periodMixingID = this.periodMixingID;
    this.updatePeriodDispatch();
  }
  NO(index) {
    return  Number(index) + 1;
    // return (this.periodsGrid.pageSettings.currentPage - 1) * this.pageSettings.pageSize + Number(index) + 1;
  }

  onChangeStartTimePeriodDispatch(value: Date): void {
    this.startTimePeriodDispatch = value;

  }
  onChangeEndTimePeriodDispatch(value: Date): void {
    this.endTimePeriodDispatch = value;

  }

  rowSelectedPeriodDispatch(args) {
    const data = args.data.entity || args.data;
    this.periodMixingID = Number(data.id);
  }
  actionBeginPeriodsDispatchGrid(args) {
    if (args.requestType === 'save') {
      if (args.action === 'add') {
        this.periodDispatch.startTime = this.startTimePeriodDispatch;
        this.periodDispatch.endTime = this.endTimePeriodDispatch;
        this.periodDispatch.periodMixingID = this.periodMixingID;
        this.createPeriodDispatch();
      }
      if (args.action === 'edit') {
        this.periodDispatch = args.data;
        this.periodDispatch.startTime = this.startTimePeriodDispatch;
        this.periodDispatch.endTime = this.endTimePeriodDispatch;
        this.periodDispatch.periodMixingID = this.periodMixingID;
        this.updatePeriodDispatch();
      }
    }
    if (args.requestType === 'delete') {
      this.deletePeriodDispatch(args.data[0].id);
    }
  }
  createPeriodDispatch() {
    this.buildingLunchTimeService.addPeriodDispatch(this.periodDispatch).subscribe(() => {
      this.alertify.success('Successfully');
      this.getPeriodDispatchByPeriodMixingID();
      this.periodDispatch = {} as PeriodDispatch;
    }, error => {
      this.alertify.error(error);
      this.getPeriodDispatchByPeriodMixingID();
    });
  }

  updatePeriodDispatch() {
    this.buildingLunchTimeService.updatePeriodDispatch(this.periodDispatch).subscribe(() => {
      this.alertify.success('Successfully');
      this.getPeriodDispatchByPeriodMixingID();
      this.periodDispatch = {} as PeriodDispatch;
    }, error => {
      this.getPeriodDispatchByPeriodMixingID();
      this.alertify.error(error);
    });
  }
  deletePeriodDispatch(id) {
    this.alertify.confirm('Delete', 'Are you sure you want to delete this "' + id + '" ?', () => {
      this.buildingLunchTimeService.deletePeriodDispatch(id).subscribe(() => {
        this.getPeriodDispatchByPeriodMixingID();
        this.alertify.success('Successfully');
      }, error => {
        this.alertify.error(error);
        this.getPeriodDispatchByPeriodMixingID();
      });
    });
  }

}

