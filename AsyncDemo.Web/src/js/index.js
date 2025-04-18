// Import libraries
import jQuery from 'jquery';
window.$ = window.jQuery = jQuery;

import 'jquery-validation';
import 'jquery-validation-unobtrusive';
import 'bootstrap/dist/js/bootstrap.bundle';
import 'datatables.net';
import 'datatables.net-bs5';
import toastr from 'toastr';
import 'toastr/build/toastr.min.css';

// Make toastr globally available
window.toastr = toastr;

// Configure toastr
toastr.options = {
    closeButton: true,
    progressBar: true,
    positionClass: 'toast-bottom-right',
    timeOut: 5000
};

// IMPORTANT: All custom scripts must be imported here to be included in the bundle
// ================================================================================

// Import custom scripts
import './site';                                     // Site-wide functionality
import './async-stats';                              // Async statistics tracking
import AsyncStatsDashboard from './async-stats-dashboard'; // Dashboard visualization
import './theme-switcher';                           // Bootswatch theme switcher

// Initialize components on document ready
jQuery(document).ready(function () {
    // Initialize DataTables
    jQuery('.table').DataTable();
    
    // Initialize async stats dashboard if container exists
    if (document.getElementById('async-stats-container')) {
        new AsyncStatsDashboard('async-stats-container');
    }
});