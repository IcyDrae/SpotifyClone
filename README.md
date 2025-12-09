# SpotifyClone

SpotifyClone is a YouTube-based music web app that mimics the look and feel of Spotify. Users can browse trending videos, play them as video, search for videos, and manage playlists saved in a MySQL database.

---

## How to Run

1. **Google API Key**

   * Go to the [Google API Console](https://console.developers.google.com/).
   * Create a new project.
   * Enable the **YouTube Data API v3**.
   * Generate an API key.

2. **Add API Key**

   * Open `appsettings.json`.
   * Add your API key:

     ```json
     "APIKey": "YOUR_API_KEY_HERE"
     ```

3. **Install MySQL**

   * Create a MySQL user:

     ```sql
     CREATE USER 'admin'@'localhost' IDENTIFIED BY 'admin';
     GRANT ALL PRIVILEGES ON *.* TO 'admin'@'localhost';
     FLUSH PRIVILEGES;
     ```

4. **Run the Application**

   ```bash
   dotnet run
   ```

   * This will create the database automatically and host the website.
   * The site will run automatically at the default URL (usually `https://localhost:5001`).

---

## Features

1. **YouTube API Integration**

   * Makes a request to the YouTube Data API on homepage load.

2. **Homepage Video Grid**

   * Shows all homepage videos with thumbnails.
   * Play videos directly in the browser.

3. **Search Functionality**

   * Search videos using a string.
   * Display search results in a responsive grid.
   * Add videos to playlists.

4. **Spotify-like UI**

   * Styled to mimic the Spotify interface.

5. **Playlist Management**

   * Save playlists with songs in the MySQL database.
   * View playlist details, showing videos in a grid.

---

## Tech Stack

* ASP.NET Core MVC
* MySQL
* Bootstrap 5
* YouTube Data API v3
* JavaScript for dynamic video rendering

---
