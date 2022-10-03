import React from 'react';
import './App.css';
import { TestCmp } from './Test';
import { NotificationContextProvider } from './signalR/NotificationContext';

function App() {
  return (
    <NotificationContextProvider hubUrl='https://localhost:7037/notificationsHub'>
      <TestCmp />
    </NotificationContextProvider>
  );
}

export default App;
