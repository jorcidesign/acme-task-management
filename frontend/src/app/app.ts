import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';

@Component({
  selector: 'app-root',
  standalone: true, // <-- ¡Esta es la pieza vital que faltaba!
  imports: [RouterOutlet],
  templateUrl: './app.html'
})
export class App { }