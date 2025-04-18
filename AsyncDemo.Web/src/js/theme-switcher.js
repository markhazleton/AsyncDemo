/**
 * Simple Theme Switcher for Bootswatch Themes
 * Completely standalone implementation that doesn't rely on Bootstrap's dropdown
 */

class ThemeSwitcher {
    constructor() {
        console.log('ThemeSwitcher initializing...');
        this.themesApiUrl = 'https://bootswatch.com/api/5.json';
        this.themeLink = document.getElementById('theme-link');
        this.themes = [];
        this.currentTheme = localStorage.getItem('selectedTheme') || 'default';
        this.isDropdownOpen = false;
        
        // Initialize the theme switcher
        this.init();
    }

    /**
     * Initialize theme switcher - fetch themes and set initial theme
     */
    async init() {
        try {
            // Set up the event handlers first
            this.setupEventHandlers();
            
            // Then fetch themes and populate dropdown
            await this.fetchThemes();
            this.populateThemeDropdown();
            
            // Apply the stored theme or default
            this.applyTheme(this.currentTheme);
            
            console.log('Theme switcher initialized successfully');
        } catch (error) {
            console.error('Error initializing theme switcher:', error);
        }
    }

    /**
     * Set up all event handlers
     */
    setupEventHandlers() {
        // Get references to DOM elements
        const themeButton = document.getElementById('theme-button');
        const themeDropdown = document.getElementById('theme-dropdown');
        
        if (!themeButton || !themeDropdown) {
            console.error('Theme button or dropdown not found');
            return;
        }
        
        // Toggle dropdown when theme button is clicked
        themeButton.addEventListener('click', (e) => {
            e.preventDefault();
            this.toggleDropdown();
        });
        
        // Close dropdown when clicking outside
        document.addEventListener('click', (e) => {
            if (!themeButton.contains(e.target) && !themeDropdown.contains(e.target)) {
                this.closeDropdown();
            }
        });
        
        console.log('Theme switcher event handlers set up');
    }
    
    /**
     * Toggle the dropdown open/closed
     */
    toggleDropdown() {
        const dropdown = document.getElementById('theme-dropdown');
        if (!dropdown) return;
        
        if (this.isDropdownOpen) {
            this.closeDropdown();
        } else {
            this.openDropdown();
        }
    }
    
    /**
     * Open the dropdown
     */
    openDropdown() {
        const dropdown = document.getElementById('theme-dropdown');
        if (!dropdown) return;
        
        dropdown.style.display = 'block';
        this.isDropdownOpen = true;
        console.log('Dropdown opened');
    }
    
    /**
     * Close the dropdown
     */
    closeDropdown() {
        const dropdown = document.getElementById('theme-dropdown');
        if (!dropdown) return;
        
        dropdown.style.display = 'none';
        this.isDropdownOpen = false;
        console.log('Dropdown closed');
    }

    /**
     * Fetch available themes from Bootswatch API
     */
    async fetchThemes() {
        try {
            console.log('Fetching themes from', this.themesApiUrl);
            const response = await fetch(this.themesApiUrl);
            if (!response.ok) {
                throw new Error(`Failed to fetch themes: ${response.status} ${response.statusText}`);
            }
            const data = await response.json();
            this.themes = data.themes;
            console.log(`Fetched ${this.themes.length} themes`);
            return this.themes;
        } catch (error) {
            console.error('Error fetching themes:', error);
            return [];
        }
    }

    /**
     * Populate the theme dropdown with available themes
     */
    populateThemeDropdown() {
        const dropdown = document.getElementById('theme-dropdown');
        if (!dropdown) {
            console.error('Theme dropdown not found');
            return;
        }

        console.log('Populating theme dropdown');
        
        // Hide dropdown by default
        dropdown.style.display = 'none';
        
        // Clear existing items
        dropdown.innerHTML = '';

        // Add default Bootstrap theme
        const defaultItem = document.createElement('a');
        defaultItem.classList.add('dropdown-item');
        defaultItem.href = '#';
        defaultItem.textContent = 'Default Bootstrap';
        defaultItem.dataset.theme = 'default';
        defaultItem.addEventListener('click', (e) => this.handleThemeSelection(e));
        dropdown.appendChild(defaultItem);

        // Add separator
        const divider = document.createElement('div');
        divider.classList.add('dropdown-divider');
        dropdown.appendChild(divider);

        // Add Bootswatch themes
        this.themes.forEach(theme => {
            const item = document.createElement('a');
            item.classList.add('dropdown-item');
            item.href = '#';
            item.textContent = theme.name;
            item.dataset.theme = theme.name.toLowerCase();
            item.dataset.themeUrl = theme.cssMin;
            item.addEventListener('click', (e) => this.handleThemeSelection(e));
            dropdown.appendChild(item);
        });
        
        console.log(`Added ${this.themes.length + 1} themes to dropdown`);
    }

    /**
     * Handle theme selection from dropdown
     */
    handleThemeSelection(event) {
        event.preventDefault();
        
        const selectedTheme = event.target.dataset.theme;
        const themeUrl = event.target.dataset.themeUrl;
        
        console.log('Theme selected:', selectedTheme);
        
        this.applyTheme(selectedTheme, themeUrl);
        this.updateActiveThemeInDropdown(selectedTheme);
        this.closeDropdown();
    }

    /**
     * Apply the selected theme
     */
    applyTheme(themeName, themeUrl) {
        if (!themeName) {
            console.error('No theme name provided');
            return;
        }
        
        // Add transition class for smooth theme changes
        document.body.classList.add('theme-transition');
        
        // Store the selection
        localStorage.setItem('selectedTheme', themeName);
        this.currentTheme = themeName;
        
        console.log('Applying theme:', themeName);
        
        // Remove any existing theme classes from body
        document.body.classList.forEach(cls => {
            if (cls.startsWith('theme-') && cls !== 'theme-transition') {
                document.body.classList.remove(cls);
            }
        });
        
        // Add theme class to body
        document.body.classList.add(`theme-${themeName.toLowerCase()}`);
        
        // Update the theme link
        if (themeName === 'default') {
            // Reset to default Bootstrap
            this.themeLink.href = 'https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css';
        } else {
            // Apply Bootswatch theme
            const theme = this.themes.find(t => t.name.toLowerCase() === themeName.toLowerCase());
            if (theme) {
                this.themeLink.href = themeUrl || theme.cssMin;
                console.log('Set theme URL to:', this.themeLink.href);
            } else {
                console.error('Theme not found:', themeName);
            }
        }

        // Update theme name in the switcher button
        const themeButton = document.getElementById('theme-button');
        if (themeButton) {
            themeButton.innerHTML = `<i class="bi bi-palette me-1"></i>${themeName === 'default' ? 'Theme' : themeName}`;
        }

        // Apply theme to custom elements that may not be affected by CSS variables
        this.applyThemeToCustomElements(themeName);

        // Dispatch a theme change event
        document.dispatchEvent(new CustomEvent('themeChanged', {
            detail: { theme: themeName }
        }));
        
        // Remove transition class after a delay to prevent transitions when not changing theme
        setTimeout(() => {
            document.body.classList.remove('theme-transition');
        }, 1000);
    }

    /**
     * Apply theme to custom elements that might not be covered by CSS variables
     */
    applyThemeToCustomElements(themeName) {
        // Apply to fade-in elements
        const fadeInElements = document.querySelectorAll('.fade-in');
        fadeInElements.forEach(el => {
            el.style.backgroundColor = getComputedStyle(document.documentElement).getPropertyValue('--fade-in-bg').trim();
        });

        // Apply to main content areas
        const contentElements = document.querySelectorAll('.container, .container-fluid, main, .content');
        contentElements.forEach(el => {
            el.style.backgroundColor = getComputedStyle(document.documentElement).getPropertyValue('--content-bg').trim();
        });

        // Apply to animations with background color cycling
        const animatedElements = document.querySelectorAll('[class*="bg-animate"]');
        if (animatedElements.length > 0) {
            const color1 = getComputedStyle(document.documentElement).getPropertyValue('--animation-color-1').trim();
            const color2 = getComputedStyle(document.documentElement).getPropertyValue('--animation-color-2').trim();
            
            // Update any inline styles or custom properties used for animations
            document.documentElement.style.setProperty('--color-cycle-1', color1);
            document.documentElement.style.setProperty('--color-cycle-2', color2);
        }

        // Apply to cards and panels
        const cards = document.querySelectorAll('.card, .panel');
        cards.forEach(card => {
            card.style.backgroundColor = getComputedStyle(document.documentElement).getPropertyValue('--card-bg').trim();
            card.style.borderColor = getComputedStyle(document.documentElement).getPropertyValue('--card-border').trim();
        });

        // Apply to stats dashboard
        const statsDashboard = document.querySelector('.async-stats-dashboard');
        if (statsDashboard) {
            statsDashboard.style.backgroundColor = getComputedStyle(document.documentElement).getPropertyValue('--stats-dashboard-bg').trim();
        }
        
        const statCards = document.querySelectorAll('.async-stat-card');
        statCards.forEach(card => {
            card.style.backgroundColor = getComputedStyle(document.documentElement).getPropertyValue('--stats-card-bg').trim();
        });
    }

    /**
     * Update active theme in dropdown (visual indicator)
     */
    updateActiveThemeInDropdown(selectedTheme) {
        const dropdown = document.getElementById('theme-dropdown');
        if (!dropdown) return;

        // Remove active class from all items
        const items = dropdown.querySelectorAll('.dropdown-item');
        items.forEach(item => {
            item.classList.remove('active');
            if (item.dataset.theme === selectedTheme) {
                item.classList.add('active');
            }
        });
    }
}

// Initialize the theme switcher when the DOM is loaded
document.addEventListener('DOMContentLoaded', () => {
    console.log('DOM loaded, creating ThemeSwitcher instance');
    window.themeSwitcher = new ThemeSwitcher();
});

// For browsers that support ES modules
if (typeof exports !== 'undefined') {
    exports.ThemeSwitcher = ThemeSwitcher;
}