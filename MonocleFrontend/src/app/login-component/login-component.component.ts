import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { AuthenticationRequest } from '../types/requests';

@Component({
  selector: 'app-login-component',
  templateUrl: './login-component.component.html',
  styleUrls: ['./login-component.component.scss']
})
export class LoginComponentComponent implements OnInit {
  loginForm!: FormGroup;

  @Output() onSubmit = new EventEmitter<AuthenticationRequest>();

  constructor(private formBuilder: FormBuilder) { }

  ngOnInit(): void {
    this.loginForm = this.formBuilder.group({
      host: ['', [Validators.required]],
      port: [55554, [Validators.required]],
      username: ['', [Validators.required]],
      password: ['', [Validators.required]],
      ssl: true,
    })
  }

  submit() {
    if (!this.loginForm.valid) {
      return;
    }

    this.onSubmit.emit(this.loginForm.value);
  }
}
