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
console.log('Copying library files...');

// Bootstrap Icons fonts
const bootstrapIconsDir = path.join(__dirname, 'node_modules', 'bootstrap-icons', 'font', 'fonts');
const bootstrapIconsDestDir = path.join(__dirname, 'wwwroot', 'fonts');

if (!fs.existsSync(bootstrapIconsDestDir)) {
  fs.mkdirSync(bootstrapIconsDestDir, { recursive: true });
}

// Copy bootstrap-icons fonts
fs.readdirSync(bootstrapIconsDir).forEach(file => {
  fs.copyFileSync(
    path.join(bootstrapIconsDir, file),
    path.join(bootstrapIconsDestDir, file)
  );
});

console.log('Build completed successfully!');