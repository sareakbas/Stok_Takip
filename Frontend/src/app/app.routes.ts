import { Routes } from '@angular/router';
import { LoginComponent } from './pages/login/login';
import { Dashboard } from './pages/dashboard/dashboard';
import { Register } from './pages/register/register';

export const routes: Routes = [
    { 
    path: '', 
    component: LoginComponent 
    },
    
    {
    path: 'dashboard',
    component: Dashboard
    },

    {
    path: 'register',
    component: Register
    }
];