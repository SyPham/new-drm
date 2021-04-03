import { Component, OnDestroy, OnInit } from '@angular/core';
import { HubConnectionState } from '@microsoft/signalr';
import * as signalr from '../../../../assets/js/ec-client.js';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
@Component({
  selector: 'app-footer',
  templateUrl: './footer.component.html',
  styleUrls: ['./footer.component.css']
})
export class FooterComponent implements OnInit {
  online: number;
  userID: number;
  userName: any;
  modalReference: any;
  constructor(public modalService: NgbModal) {
    this.userName = JSON.parse(localStorage.getItem('user')).User.Username;
    this.userID = +JSON.parse(localStorage.getItem('user')).User.ID;
  }
  ngOnInit(): void {
    if (signalr.CONNECTION_HUB.state === HubConnectionState.Connected) {
      signalr.CONNECTION_HUB
        .invoke('CheckOnline', this.userID, this.userName)
        .catch(error => {
          console.log(`CheckOnline error: ${error}`);
        }
        );
      signalr.CONNECTION_HUB.on('Online', (users) => {
        this.online = users;
      });

      signalr.CONNECTION_HUB.on('UserOnline', (userNames: any) => {
        const userNameList = JSON.stringify(userNames);
        localStorage.setItem('userOnline', userNameList);
      });
    }
  }
  openModal(ref) {
    this.modalReference = this.modalService.open(ref, { size: 'xl', backdrop: 'static', keyboard: false });

  }
}
