import { Component, ElementRef, Input, OnInit, ViewChild } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { CvService } from './../services/cv/cv.service';

@Component({
  selector: 'app-add-application',
  templateUrl: './add-application.component.html',
  styleUrls: ['./add-application.component.css']
})
export class AddApplicationComponent implements OnInit {

  @Input()
  accountStatus: string;
  educationLevel: any[] =
  [
    {display:'Elementary school', value: 1},
    {display:'High school', value: 2},
    {display:'Bachelors degree', value: 3},
    {display:'Masters degree', value: 4},
    {display:'PhD degree', value: 5}
  ];

  editUserForm = new FormGroup({
    name : new FormControl('', Validators.required),
    lastname: new FormControl('', Validators.required),
    username: new FormControl('', Validators.required),
    email: new FormControl('', Validators.required),
    imageURL : new FormControl(null),
    role: new FormControl('', Validators.nullValidator)
  })

  imagePreview:string = '';
  isLoading:boolean = true;
  @ViewChild('closeBtn') closeBtn: ElementRef;
  @ViewChild('resetBtn') resetBtn: ElementRef;

  constructor(private router:Router, private cvService:CvService) { }

  ngOnInit(): void {

  }


  saveChanges()
  {
    
  }

}
