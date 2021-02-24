import { Component, OnDestroy, OnInit } from '@angular/core';
import { HubConnectionState } from '@microsoft/signalr';
import { SignalrService } from 'src/app/_core/_service/signalr.service.js';
import * as signalr from '../../../../assets/js/ec-client.js';

@Component({
  selector: 'app-footer',
  templateUrl: './footer.component.html',
  styleUrls: ['./footer.component.css'],
  providers: [SignalrService]
})
export class FooterComponent implements OnInit, OnDestroy {
  online: number;
  userID: number;
  constructor(public signalrService: SignalrService) {
    this.userID = +JSON.parse(localStorage.getItem('user')).User.ID;
  }
  ngOnDestroy(): void {
    this.signalrService.close();
  }
  ngOnInit(): void {
    this.signalrService.connect();
    this.signalrService.online.subscribe((users: any) => {
      this.online = users || 0;
    });
    // if (signalr.CONNECTION_HUB.state === 'Connected') {
    //   signalr.CONNECTION_HUB.invoke('CheckOnline', this.userID).catch(err => console.error(err));
    //   signalr.CONNECTION_HUB.on('Online', (users) => {
    //     this.online = users;
    //   });
    // }
  }
}
