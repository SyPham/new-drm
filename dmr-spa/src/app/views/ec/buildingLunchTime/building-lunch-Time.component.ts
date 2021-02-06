import { Component, OnInit } from '@angular/core';
import { BuildingUserService } from 'src/app/_core/_service/building.user.service';
import { AlertifyService } from 'src/app/_core/_service/alertify.service';
import { LunchTime } from 'src/app/_core/_model/building';
import { NgbModal, NgbModalRef } from '@ng-bootstrap/ng-bootstrap';
import { BuildingLunchTimeService } from 'src/app/_core/_service/building.lunch.time.service';

@Component({
  selector: 'app-building-lunch-time',
  templateUrl: './building-lunch-Time.component.html',
  styleUrls: ['./building-lunch-Time.component.scss']
})
export class BuildingLunchTimeComponent implements OnInit {

  toolbar: object;
  data: any;
  periods = [];
  buildingID: number;
  userData: any;
  buildingUserData: any;
  buildings: any;
  steps = {hour: 1, minute: 30 };
  modalReference: NgbModalRef;
  toolbarOptions = ['Search'];
  fields: object = { text: 'content', value: 'content' };
  lunchTimeData = new LunchTime().data;
  editSettings: { showDeleteConfirmDialog: boolean; allowEditing: boolean; allowAdding: boolean; allowDeleting: boolean; mode: string; };
  editSettingsPeriod: { showDeleteConfirmDialog: boolean;
    allowEditing: boolean; allowAdding: boolean; allowDeleting: boolean; mode: string; };
  startTime: Date;
  endTime: Date;
  lunchTimeID: number;
  constructor(
    private buildingLunchTimeService: BuildingLunchTimeService,
    private alertify: AlertifyService,
    public modalService: NgbModal,
  ) { }

  ngOnInit() {
    this.editSettingsPeriod = { showDeleteConfirmDialog: false,
      allowEditing: true, allowAdding: true, allowDeleting: true, mode: 'Normal' };
    this.editSettings = { showDeleteConfirmDialog: false, allowEditing: true, allowAdding: true, allowDeleting: true, mode: 'Normal' };
    this.toolbar = ['Search'];
  }
  created() {
    this.getBuildings();
  }
  rowSelected(args) {
    const data = args.data.entity || args.data;
    this.buildingID = Number(data.id);
  }
  actionBeginPeriodsGrid(args){
    if (args.requestType === 'beginEdit') {
      const data = args.rowData;
      this.startTime = data.startTime;
      this.endTime = data.endTime ;
    }
    if (args.requestType === 'save' && args.action === 'edit') {
      const data = args.data;
      const updating = {...data};
      updating.startTime = this.startTime;
      updating.endTime = this.endTime;
      updating.lunchTimeID = this.lunchTimeID;
      delete updating.period;
      console.log('edit', updating);
      this.updatePeriod(updating);
    }
  }
  onChangeStartTime(value: Date): void {
    this.startTime = value;

  }
  onChangeEndTime(value: Date): void {
    this.endTime = value;

  }
  onSelectLunchTime(args) {
    const item = {
      startTime: args.itemData.startTime,
      endTime: args.itemData.endTime,
      buildingID: this.buildingID
    };
    this.buildingLunchTimeService.addOrUpdateLunchTime(item).subscribe(() => {
      this.alertify.success('success!');
      this.getBuildings();
    }, err => {
      this.alertify.error(err);
    });
  }
  updatePeriod(item) {
    this.buildingLunchTimeService.updatePeriod(item).subscribe((result: any) => {
      this.getAllPeriodByLunchTime();
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
  getAllPeriodByLunchTime() {
    this.buildingLunchTimeService.getAllPeriodByLunchTime(this.lunchTimeID).subscribe((res: any) => {
      this.periods = res.map(item => {
        return {
          sequence: item.sequence,
          startTime: new Date(item.startTime),
          endTime: new Date(item.endTime),
          id: item.id
        };
      });
    });
  }
  showModal(name, value) {
    this.lunchTimeID = value.lunchTimeID;
    this.getAllPeriodByLunchTime();
    this.modalReference = this.modalService.open(name, { size: 'md' });
  }
}

