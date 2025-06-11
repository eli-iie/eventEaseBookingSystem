// Quick Application Health Check Script
// Run this in browser console on http://localhost:5104

console.log("🚀 EventEase Booking System - Health Check Started");

async function runHealthCheck() {
    const results = {
        navigation: {},
        api: {},
        ui: {}
    };

    // Test 1: Check if main navigation cards are present
    console.log("📋 Testing Navigation Cards...");
    const navCards = document.querySelectorAll('.card');
    results.navigation.cardsPresent = navCards.length >= 4;
    results.navigation.cardCount = navCards.length;
    
    // Test 2: Check if Bootstrap is loaded
    console.log("🎨 Testing Bootstrap CSS...");
    results.ui.bootstrapLoaded = !!document.querySelector('.container, .row, .col');
    
    // Test 3: Check if Font Awesome icons are present
    console.log("🎯 Testing Font Awesome Icons...");
    results.ui.fontAwesomeLoaded = !!document.querySelector('i[class*="fa-"]');
    
    // Test 4: Test page responsiveness
    console.log("📱 Testing Responsive Design...");
    const viewportMeta = document.querySelector('meta[name="viewport"]');
    results.ui.responsiveReady = !!viewportMeta;
    
    // Test 5: Check for navigation links
    console.log("🔗 Testing Navigation Links...");
    const navLinks = document.querySelectorAll('a[href]');
    results.navigation.linksCount = navLinks.length;
    results.navigation.hasVenuesLink = !!document.querySelector('a[href*="Venues"]');
    results.navigation.hasEventsLink = !!document.querySelector('a[href*="Events"]');
    results.navigation.hasBookingsLink = !!document.querySelector('a[href*="Bookings"]');
    results.navigation.hasReportsLink = !!document.querySelector('a[href*="Reports"]');
    
    // Test 6: Check for main content areas
    console.log("📄 Testing Page Structure...");
    results.ui.hasMainContent = !!document.querySelector('main, .main-content, .container');
    results.ui.hasHeader = !!document.querySelector('header, nav, .navbar');
    
    // Summary
    console.log("✅ Health Check Completed!");
    console.log("📊 Results:", results);
    
    // Calculate overall health score
    let passedTests = 0;
    let totalTests = 0;
    
    Object.values(results).forEach(category => {
        Object.values(category).forEach(test => {
            totalTests++;
            if (test === true || (typeof test === 'number' && test > 0)) {
                passedTests++;
            }
        });
    });
    
    const healthScore = Math.round((passedTests / totalTests) * 100);
    console.log(`🏆 Overall Health Score: ${healthScore}% (${passedTests}/${totalTests} tests passed)`);
    
    if (healthScore >= 80) {
        console.log("🟢 System Status: HEALTHY");
    } else if (healthScore >= 60) {
        console.log("🟡 System Status: WARNING - Some issues detected");
    } else {
        console.log("🔴 System Status: CRITICAL - Multiple issues detected");
    }
    
    return results;
}

// Auto-run the health check
runHealthCheck();

// Provide manual test functions
window.EventEaseTests = {
    checkHealth: runHealthCheck,
    
    // Test navigation to different sections
    testNavigation: async function() {
        console.log("🧭 Testing Navigation...");
        const links = ['Venues', 'Events', 'Bookings', 'Reports'];
        
        for (const link of links) {
            const element = document.querySelector(`a[href*="${link}"]`);
            if (element) {
                console.log(`✅ ${link} link found`);
            } else {
                console.log(`❌ ${link} link missing`);
            }
        }
    },
    
    // Test form validation
    testForms: function() {
        console.log("📝 Testing Forms...");
        const forms = document.querySelectorAll('form');
        console.log(`Found ${forms.length} forms on current page`);
        
        forms.forEach((form, index) => {
            const inputs = form.querySelectorAll('input[required], select[required], textarea[required]');
            console.log(`Form ${index + 1}: ${inputs.length} required fields`);
        });
    },
    
    // Test accessibility
    testAccessibility: function() {
        console.log("♿ Testing Accessibility...");
        const results = {
            altTexts: document.querySelectorAll('img[alt]').length,
            labels: document.querySelectorAll('label').length,
            headings: document.querySelectorAll('h1, h2, h3, h4, h5, h6').length,
            skipLinks: document.querySelectorAll('a[href="#main"], a[href="#content"]').length
        };
        console.log("Accessibility features:", results);
    }
};

console.log("🔧 Manual test functions available as window.EventEaseTests");
console.log("   - EventEaseTests.checkHealth()");
console.log("   - EventEaseTests.testNavigation()");
console.log("   - EventEaseTests.testForms()");
console.log("   - EventEaseTests.testAccessibility()");
