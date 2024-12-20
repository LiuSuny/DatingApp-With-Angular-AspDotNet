import { Component, OnInit } from '@angular/core';
import { MessageService } from '../_services/message.service';
import { Message } from '../_models/message';
import { Pagination } from '../_models/pagination';
import { ConfirmService } from '../_services/confirm.service';


@Component({
  selector: 'app-messages',
  templateUrl: './messages.component.html',
  styleUrls: ['./messages.component.css']
})
export class MessagesComponent implements OnInit{

  messages: Message[] = [];
  pagination: Pagination;
  container = 'Unread';
  pageNumber = 1;
  pageSize = 5;
 //adding a load flag as this allow thing to be ready before loading it
 loading = false;

  constructor(private messageService: MessageService, private confirmService: ConfirmService){}

  ngOnInit(): void {
    this.loadMessages();
  }
  
  loadMessages(){
    this.loading= true;
    this.messageService.getMessages
    ( this.pageNumber, this.pageSize, this.container)
    .subscribe(response => {
      this.messages = response.result; //our member is store in result inside our model PaginationResult model
       this.pagination = response.pagination;
       this.loading = false;
     })
  }


  deleteMessage(id: number){
    this.confirmService.confirm('Confirm delete message ', ' this cannot be undone')
    .subscribe(result => {
      if(result)
      {
        this.messageService.deleteMessage(id).subscribe(() => {
          this.messages.splice(this.messages.findIndex(m => m.id === id), 1);
         ;
        })
      }    
    });
    
  }
  pageChanged(event: any){
    if(this.pageNumber !== event.page)
    {
      this.pageNumber = event.page;
      this.loadMessages();
    }     
  }

}
