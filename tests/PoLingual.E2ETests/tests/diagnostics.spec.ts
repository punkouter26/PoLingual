import { test, expect } from '@playwright/test';

test.describe('Diagnostics Page', () => {
  test('should display diagnostics heading', async ({ page }) => {
    await page.goto('/diag');
    await expect(page.locator('text=Diagnostics')).toBeVisible();
  });

  test('should have run checks button', async ({ page }) => {
    await page.goto('/diag');
    await expect(page.locator('text=Run All Checks')).toBeVisible();
  });
});
