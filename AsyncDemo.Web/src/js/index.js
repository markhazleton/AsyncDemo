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

// Import custom scripts
import './site';
import './async-stats';
import AsyncStatsDashboard from './async-stats-dashboard';

// Initialize components on document ready
jQuery(document).ready(function () {
    // Initialize DataTables
    jQuery('.table').DataTable();
    
    // Initialize async stats dashboard if container exists
    if (document.getElementById('async-stats-container')) {
        new AsyncStatsDashboard('async-stats-container');
    }
});