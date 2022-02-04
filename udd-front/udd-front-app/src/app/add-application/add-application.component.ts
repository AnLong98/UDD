import { Component,  Input, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { City } from '../models/city.model';
import { CvService } from './../services/cv/cv.service';
import { ValidationService } from './../services/cv/validation.service';
import { CityService } from './../services/cv/city.service';

@Component({
  selector: 'app-add-application',
  templateUrl: './add-application.component.html',
  styleUrls: ['./add-application.component.css']
})
export class AddApplicationComponent implements OnInit {

  @Input()
  accountStatus: string;
  educationLevels: any[] =
  [
    {display:'Elementary school', value: 1},
    {display:'High school', value: 2},
    {display:'Bachelors degree', value: 3},
    {display:'Masters degree', value: 4},
    {display:'PhD degree', value: 5}
  ];

  jobAppForm = new FormGroup({
    name : new FormControl('', Validators.required),
    lastname: new FormControl('', Validators.required),
    city: new FormControl('', Validators.required),
    cvFile: new FormControl(null, Validators.required),
    coverLetter : new FormControl(null,  Validators.required),
    educationLevel: new FormControl('1', Validators.nullValidator),
    cvFileSource: new FormControl('', Validators.required),
    letterSource: new FormControl('', Validators.required),
  })
  cities: City[] = [];
  isLoading:boolean = false;

  constructor(private router:Router, private cvService:CvService, private validation:ValidationService, private cityService:CityService) { }

  ngOnInit(): void {
    this.isLoading = true;
    this.loadCities();

  }

  loadCities(){
    this.cityService.getCities()
    .subscribe(
      data =>{
        this.cities = data
        this.isLoading = false;
      },
      error =>{
        this.isLoading = false;
      }
    )
  }

  get f(){
    return this.jobAppForm.controls;
  }

  onCvFileChange(event) {
  
    if (event.target.files.length > 0) {
      const file = event.target.files[0];
      this.jobAppForm.patchValue({
        cvFileSource: file
      });
    }
  }

  onLetterFileChange(event) {
  
    if (event.target.files.length > 0) {
      const file = event.target.files[0];
      this.jobAppForm.patchValue({
        letterSource: file
      });
    }
  }
     
     
  saveChanges()
  {
    if(!this.jobAppForm.valid)
      {
        this.validation.validateAllFields(this.jobAppForm);
        return;
      }
    let name = this.jobAppForm.controls['name'].value;
    let lastName = this.jobAppForm.controls['lastname'].value;
    let city = this.jobAppForm.controls['city'].value;
    let educationLevel = this.jobAppForm.controls['educationLevel'].value;
    let cvFile = this.jobAppForm.controls['cvFileSource'].value;
    let letterFile = this.jobAppForm.controls['letterSource'].value;
    this.isLoading = true;
    this.cvService.addNewJobApplication(name, lastName, city, educationLevel, cvFile, letterFile).subscribe(
        data=>{
          this.isLoading = false;
          alert("File uploaded.");
          this.router.navigate(['/']);
        },

        error=>{
          this.isLoading = false;
          alert("File not uploaded.");
        }
    );

  }

}
