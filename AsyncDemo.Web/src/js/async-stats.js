/**
 * AsyncStats - A utility for tracking asynchronous operations and displaying toast notifications.
 * This module provides functionality to track async operations, their duration, and display
 * toast notifications when they complete.
 */

class AsyncStats {
    constructor() {
        this.operations = {};
        this.history = [];
        this.operationCount = 0;
        this.successCount = 0;
        this.failureCount = 0;
        this.fastestOperation = null;
        this.slowestOperation = null;
        this.totalDuration = 0;
        
        // Initialize the toast options if toastr is available
        if (window.toastr) {
            toastr.options = {
                closeButton: true,
                progressBar: true,
                positionClass: 'toast-bottom-right',
                preventDuplicates: false,
                timeOut: 5000
            };
        }
    }

    /**
     * Start tracking a new async operation
     * @param {string} name - Name of the operation
     * @returns {string} - Operation ID that can be used to complete the operation
     */
    startOperation(name) {
        const operationId = `op_${Date.now()}_${Math.random().toString(36).substr(2, 9)}`;
        this.operations[operationId] = {
            id: operationId,
            name: name,
            startTime: Date.now(),
            endTime: null,
            duration: null,
            status: 'pending',
            message: null
        };
        
        return operationId;
    }

    /**
     * Complete a tracked async operation and show a toast notification
     * @param {string} operationId - The operation ID returned from startOperation
     * @param {boolean} success - Whether the operation was successful
     * @param {string} message - Message to display in the toast notification
     */
    completeOperation(operationId, success, message) {
        if (!this.operations[operationId]) {
            console.warn(`Operation ${operationId} not found.`);
            return;
        }
        
        const endTime = Date.now();
        const operation = this.operations[operationId];
        operation.endTime = endTime;
        operation.duration = endTime - operation.startTime;
        operation.status = success ? 'success' : 'failed';
        operation.message = message || (success ? 'Operation completed successfully' : 'Operation failed');
        
        // Update statistics
        this.operationCount++;
        if (success) {
            this.successCount++;
        } else {
            this.failureCount++;
        }
        
        // Track fastest and slowest operations
        if (!this.fastestOperation || operation.duration < this.fastestOperation.duration) {
            this.fastestOperation = { ...operation };
        }
        
        if (!this.slowestOperation || operation.duration > this.slowestOperation.duration) {
            this.slowestOperation = { ...operation };
        }
        
        // Update total duration for average calculation
        this.totalDuration += operation.duration;
        
        // Add to history for future reference
        this.history.push({ ...operation });
        
        // Limit history to last 100 operations
        if (this.history.length > 100) {
            this.history.shift();
        }
        
        // Show toast notification if toastr is available
        this.showToast(operation);
        
        return operation;
    }

    /**
     * Show a toast notification for the completed operation
     * @param {Object} operation - The completed operation
     */
    showToast(operation) {
        if (!window.toastr) return;
        
        // Determine duration class for styling
        let durationClass = 'duration-fast';
        if (operation.duration > 1000) {
            durationClass = 'duration-slow';
        } else if (operation.duration > 500) {
            durationClass = 'duration-medium';
        }
        
        // Create custom toast content
        const toastContent = `
            <div class="async-toast">
                <div class="async-toast-header">${operation.name}</div>
                <div class="async-toast-body">
                    <div class="async-stat">
                        <i class="bi bi-check-circle-fill text-success"></i>
                        <span>${operation.message}</span>
                    </div>
                    <div class="async-stat">
                        <i class="bi bi-clock"></i>
                        <span class="${durationClass}">${operation.duration} ms</span>
                    </div>
                </div>
            </div>
        `;
        
        // Show toast based on operation status
        if (operation.status === 'success') {
            toastr.success(toastContent, '', { timeOut: 5000, escapeHtml: false });
        } else {
            toastr.error(toastContent, '', { timeOut: 8000, escapeHtml: false });
        }
    }

    /**
     * Get the current statistics for all tracked operations
     * @returns {Object} - Statistics object
     */
    getStats() {
        return {
            totalOperations: this.operationCount,
            successfulOperations: this.successCount,
            failedOperations: this.failureCount,
            averageTime: this.operationCount > 0 ? this.totalDuration / this.operationCount : 0,
            successRate: this.operationCount > 0 ? Math.round((this.successCount / this.operationCount) * 100) : 100,
            fastestOperation: this.fastestOperation,
            slowestOperation: this.slowestOperation,
            recentOperations: this.history.slice(-10) // Last 10 operations
        };
    }
}

// Create a global instance of AsyncStats
window.asyncStats = new AsyncStats();

export default window.asyncStats;