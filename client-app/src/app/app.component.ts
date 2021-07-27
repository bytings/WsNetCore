import { Component, resolveForwardRef } from '@angular/core';
import { WebSocketService } from './web-socket.service';
import { Square } from './models/square';
import { SquareChangeRequest } from './models/square-change-request';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent {
  announcementSub;
  messages: string[] = [];
  squares: Square[] = [];
  accion: string[] = ["Abrir", "Cerrar"];
  currentAccion: string = "Abrir";
  currentX: number = 100;
  currentY: number = 200;
  currentHe: number = 400;
  currentWe: number = 600;
  name: string = "";
  constructor(private socketService: WebSocketService) {
    this.socketService.announcement$.subscribe(announcement => {
      if (announcement) {
        this.messages.unshift(announcement);
      }
    });
    this.socketService.name$.subscribe(n => {
      this.name = n;
    });
  }

  ngOnInit() {
    this.socketService.startSocket();

  }

  Click() {
    var req = new SquareChangeRequest();
    req.X = this.currentX;
    req.Y = this.currentY;
    req.Height = this.currentHe;
    req.Width = this.currentWe;
    req.close = false;
    if(this.currentAccion=="Cerrar")
    {
      req.close = true;
    }

    this.socketService.sendChangeRequest(req);

  }
}
