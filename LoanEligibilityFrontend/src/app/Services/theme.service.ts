import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class ThemeService {

  toggleTheme() {
    const body = document.body;

    if (body.classList.contains('dark-theme')) {
      body.classList.remove('dark-theme');
      body.classList.add('light-theme');
      localStorage.setItem('theme', 'light');
    } else {
      body.classList.remove('light-theme');
      body.classList.add('dark-theme');
      localStorage.setItem('theme', 'dark');
    }
  }

  loadTheme() {
    const saved = localStorage.getItem('theme') || 'dark';

    document.body.classList.add(saved === 'dark' ? 'dark-theme' : 'light-theme');
  }
}
