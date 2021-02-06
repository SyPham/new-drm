import { Component, OnInit } from '@angular/core';
import * as signalr from '../../../../assets/js/ec-client.js';

@Component({
  selector: 'app-footer',
  templateUrl: './footer.component.html',
  styleUrls: ['./footer.component.css']
})
export class FooterComponent implements OnInit {
  online: number;
  userID: number;
  constructor() {
    this.userID = +JSON.parse(localStorage.getItem('user')).User.ID;
   }
  ngOnInit(): void {
    if (signalr.CONNECTION_HUB.state === 'Connected') {
      signalr.CONNECTION_HUB.invoke('CheckOnline', this.userID).catch(err => console.error(err));
      signalr.CONNECTION_HUB.on('Online', (users) => {
        this.online = users;
      });
    }
  }

}
