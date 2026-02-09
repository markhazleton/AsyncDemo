const fs = require('fs');
const path = require('path');
const childProcess = require('child_process');

// Run webpack to bundle the assets
console.log('Bundling JavaScript and CSS files...');
childProcess.execSync('npx webpack --config webpack.config.js', { stdio: 'inherit' });

// Ensure the lib directory exists
const libDir = path.join(__dirname, 'wwwroot', 'lib');

// Check if the directory exists
if (!fs.existsSync(libDir)) {
  fs.mkdirSync(libDir, { recursive: true });
}

// Copy specific vendor files that should be available directly
// Note: Bootstrap Icons are loaded from CDN, no local copy needed

console.log('Build completed successfully!');