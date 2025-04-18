/**
 * AsyncStatsDashboard - A component for displaying async operation statistics
 */
class AsyncStatsDashboard {
    constructor(containerId) {
        this.containerId = containerId;
        this.container = document.getElementById(containerId);
        if (!this.container) {
            console.error(`Container with ID ${containerId} not found`);
            return;
        }
        this.render();
    }

    /**
     * Render the stats dashboard
     */
    render() {
        if (!this.container) return;
        
        // Create dashboard structure
        const dashboard = document.createElement('div');
        dashboard.className = 'async-stats-dashboard';
        dashboard.innerHTML = `
            <div class="async-stats-header">
                <i class="bi bi-speedometer2"></i>
                <span>Async Operations Statistics</span>
            </div>
            <div class="async-stats-grid">
                <div class="async-stat-card">
                    <div class="stat-label">Total Operations</div>
                    <div class="stat-value" id="stat-total">0</div>
                </div>
                <div class="async-stat-card">
                    <div class="stat-label">Successful</div>
                    <div class="stat-value success" id="stat-success">0</div>
                </div>
                <div class="async-stat-card">
                    <div class="stat-label">Failed</div>
                    <div class="stat-value danger" id="stat-failed">0</div>
                </div>
                <div class="async-stat-card">
                    <div class="stat-label">Average Time</div>
                    <div class="stat-value info" id="stat-avg-time">0 ms</div>
                </div>
                <div class="async-stat-card">
                    <div class="stat-label">Success Rate</div>
                    <div class="stat-value" id="stat-success-rate">100%</div>
                </div>
                <div class="async-stat-card">
                    <div class="stat-label">Fastest Operation</div>
                    <div class="stat-value success" id="stat-fastest">N/A</div>
                </div>
                <div class="async-stat-card">
                    <div class="stat-label">Slowest Operation</div>
                    <div class="stat-value warning" id="stat-slowest">N/A</div>
                </div>
            </div>
        `;
        
        this.container.appendChild(dashboard);
        
        // Start updating the stats
        this.startUpdating();
    }

    /**
     * Start updating stats periodically
     */
    startUpdating() {
        // Update immediately
        this.updateStats();
        
        // Then update every 2 seconds
        setInterval(() => this.updateStats(), 2000);
    }

    /**
     * Update stats with the latest values
     */
    updateStats() {
        if (!window.asyncStats) return;
        
        const stats = window.asyncStats.getStats();
        
        // Update the stats values
        document.getElementById('stat-total').textContent = stats.totalOperations;
        document.getElementById('stat-success').textContent = stats.successfulOperations;
        document.getElementById('stat-failed').textContent = stats.failedOperations;
        document.getElementById('stat-avg-time').textContent = `${stats.averageTime.toFixed(2)} ms`;
        document.getElementById('stat-success-rate').textContent = `${stats.successRate}%`;
        
        if (stats.fastestOperation) {
            document.getElementById('stat-fastest').textContent = 
                `${stats.fastestOperation.duration.toFixed(2)} ms (${stats.fastestOperation.name})`;
        }
        
        if (stats.slowestOperation) {
            document.getElementById('stat-slowest').textContent = 
                `${stats.slowestOperation.duration.toFixed(2)} ms (${stats.slowestOperation.name})`;
        }
    }
}

export default AsyncStatsDashboard;