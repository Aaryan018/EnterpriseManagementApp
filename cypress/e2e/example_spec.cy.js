// cypress/integration/example_spec.js

describe('My First Test', () => {
    it('Visits the Cypress website', () => {
        cy.visit('https://www.cypress.io')
        cy.contains('Cypress').click()
        cy.url().should('include', 'cypress.io')
    })
})
