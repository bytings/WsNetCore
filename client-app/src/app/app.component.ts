import { FormsModule } from '@angular/forms';
import { Component, Input, resolveForwardRef } from '@angular/core';
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
  messages: any[] = [];
  squares: Square[] = [];
  accion: string[] = ["Abrir", "Cerrar"];
  currentAccion: string = "Abrir";
  currentX: number = 0;
  currentY: number = 236;
  currentHe: number = 400;
  currentWe: number = 600;
  name: string = "";

  constructor(private socketService: WebSocketService) {
    this.socketService.announcement$.subscribe(announcement => {
      if (announcement && announcement[0]!='no') {
        this.messages.unshift(announcement[0]);
      }
      else{
        this.currentX = announcement[1];
        this.currentY = announcement[2];
        this.currentHe = announcement[4];
        this.currentWe = announcement[3];
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
    req.Width = this.currentHe;
    req.Height = this.currentWe;
    req.close = false;
    if(this.currentAccion=="Cerrar")
    {
      req.close = true;
    }

    this.socketService.sendChangeRequest(req);

  }

  onDragMove(event) {
    let element = event.source.getRootElement();
    let boundingClientRect = element.getBoundingClientRect();
    this.currentX = boundingClientRect.x;
    this.currentY = boundingClientRect.y;
    let parentPosition = this.getPosition(element);
    console.log('x: ' + (boundingClientRect.x - parentPosition.left), 'y: ' + (boundingClientRect.y - parentPosition.top));

    var req = new SquareChangeRequest();
    req.X = this.currentX;
    req.Y = this.currentY;
    req.Width = this.currentHe;
    req.Height = this.currentWe;
    req.close = false;
    if(this.currentAccion=="Cerrar")
    {
      req.close = true;
    }

    this.socketService.sendChangeRequest(req);
  }

  getPosition(el) {
    let x = 0;
    let y = 0;
    while(el && !isNaN(el.offsetLeft) && !isNaN(el.offsetTop)) {
      x += el.offsetLeft - el.scrollLeft;
      y += el.offsetTop - el.scrollTop;
      el = el.offsetParent;
    }
    return { top: y, left: x };
  }


}
