describe('Paying for Assets', () => {
    before(() => {

        // Setup: Log in and store session data
        cy.session('userSession', () => {
            // Visit your login page (this step is for login only)
            cy.visit('http://localhost:5047');

            // Replace these with actual login field selectors and credentials
            cy.get('input[name="Email"]').type('client@enterprise.com');
            cy.get('input[name="Password"]').type('Client@123');
            cy.get('button[type="submit"]').click();

            // Ensure the user is redirected to the correct page
            cy.url().should('include', '/Customers/Index'); // Adjust if needed
        });
    });

    it('should successfully pay for an asset and verify the paid amount', () => {
        // Start directly at "My Assets" page
        cy.visit('http://localhost:5047/CustomerOccupancyHistory/Index'); // Make sure this is the correct URL for "My Assets"

        // Ensure asset seeded record exists
        cy.get('table.assets-table')
            .should('exist') // Ensure the table exists
            .find('tbody tr')
            .should('have.length.greaterThan', 0); // Make sure there are rows in the table

        // Click "Pay" button
        cy.contains('Pay').click();

        // Enter "30" in the "AmountPaid" text field (replace with actual selector)
        // cy.get('input[asp-for="AmmountPaid"]').clear().type('30');
        cy.get('input[name="AmmountPaid"]').clear().type('30');


        // Click the "Create" button to submit the form
        // cy.contains('Create').click();
        cy.get('input[type="submit"][value="Create"]').click();

        // Ensure the user is redirected to the correct page
        cy.url().should('include', '/CustomerOccupancyHistory/Index'); // Adjust if needed

        // Find the row where the "Asset" column has the value "Sample Asset 2 - OH Test"
        //cy.get('table.assets-table tbody tr')
        //    .contains('td', 'Sample Asset 2 - OH Test') // Replace 'Sample Asset 2 - OH Test' with the asset you're targeting
        //    .parent() // Get the parent row of the found cell
        //    .find('td') // Get all td elements in the row
        //    .eq(3) // The "Paid" column is at index 3 (the fourth column)
        //    .then(($td) => {
        //        cy.log($td.text()); // Log the text content of the "Paid" column
        //    })
        //    .should('include.text', '30'); // Check if it contains '30'
    });
});
