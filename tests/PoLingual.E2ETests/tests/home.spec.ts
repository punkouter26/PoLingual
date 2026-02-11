import { test, expect } from '@playwright/test';

test.describe('Home Page', () => {
  test('should display the PoLingual title', async ({ page }) => {
    await page.goto('/');
    await expect(page.locator('text=PoLingual')).toBeVisible();
  });

  test('should have navigation links', async ({ page }) => {
    await page.goto('/');
    await expect(page.locator('text=Leaderboard')).toBeVisible();
    await expect(page.locator('text=Translate')).toBeVisible();
    await expect(page.locator('text=Diagnostics')).toBeVisible();
  });

  test('should navigate to leaderboard', async ({ page }) => {
    await page.goto('/');
    await page.click('text=Leaderboard');
    await expect(page).toHaveURL(/.*leaderboard/);
  });

  test('should navigate to translate', async ({ page }) => {
    await page.goto('/');
    await page.click('text=Translate');
    await expect(page).toHaveURL(/.*translate/);
  });

  test('should navigate to diagnostics', async ({ page }) => {
    await page.goto('/');
    await page.click('text=Diagnostics');
    await expect(page).toHaveURL(/.*diag/);
  });
});
