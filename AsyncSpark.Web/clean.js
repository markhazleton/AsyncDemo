/**
 * Clean script to remove generated files from output directories
 * This script can be run with 'npm run clean'
 */
const fs = require('fs');
const path = require('path');

// Directories to clean
const directoriesToClean = [
  // wwwroot output directories
  path.join(__dirname, 'wwwroot', 'js'),
  path.join(__dirname, 'wwwroot', 'css'),
  path.join(__dirname, 'wwwroot', 'fonts'),
  // Any other directories you might want to clean
];

// Files to preserve (won't be deleted even if in target directories)
const filesToPreserve = [
  '.gitkeep',
  'README.md'
];

console.log('Cleaning output directories...');

// Function to empty a directory without deleting the directory itself
function cleanDirectory(directory) {
  if (!fs.existsSync(directory)) {
    console.log(`Directory does not exist: ${directory}`);
    return;
  }

  console.log(`Cleaning directory: ${directory}`);
  
  const files = fs.readdirSync(directory);
  
  for (const file of files) {
    const filePath = path.join(directory, file);
    
    // Skip files that should be preserved
    if (filesToPreserve.includes(file)) {
      console.log(`  Preserving: ${file}`);
      continue;
    }
    
    const stat = fs.statSync(filePath);
    
    if (stat.isDirectory()) {
      // Recursively clean subdirectories
      cleanDirectory(filePath);
      
      // Remove empty directory
      if (fs.readdirSync(filePath).length === 0) {
        fs.rmdirSync(filePath);
        console.log(`  Removed empty directory: ${file}`);
      }
    } else {
      // Delete file
      fs.unlinkSync(filePath);
      console.log(`  Deleted: ${file}`);
    }
  }
}

// Clean each directory
for (const directory of directoriesToClean) {
  cleanDirectory(directory);
}

console.log('Cleaning completed successfully!');