import { Component, Input, OnInit } from '@angular/core';
import { JobApplication } from './../models/job-application.model';
import { SearchResult } from './../models/content-search-result.model';

@Component({
  selector: 'app-document-card',
  templateUrl: './document-card.component.html',
  styleUrls: ['./document-card.component.css']
})
export class DocumentCardComponent implements OnInit {

  @Input()
  application:SearchResult;
  
  constructor() { }

  ngOnInit(): void {
  }

}
