import { Component, Input, OnInit } from '@angular/core';
import { SearchResult } from './../models/content-search-result.model';
import { CvService } from './../services/cv/cv.service';
import * as fileSaver from 'file-saver';

@Component({
  selector: 'app-document-card',
  templateUrl: './document-card.component.html',
  styleUrls: ['./document-card.component.css']
})
export class DocumentCardComponent implements OnInit {

  @Input()
  application:SearchResult;
  
  constructor(private cvService:CvService) { }

  ngOnInit(): void {
  }

  downloadFile() {
		this.cvService.downloadAttachment(this.application.jobApplication.id).subscribe(
      (response: Blob) => { 
      fileSaver.saveAs(response, "download");
		},
     error =>
     {

     }
    )
  }

}
