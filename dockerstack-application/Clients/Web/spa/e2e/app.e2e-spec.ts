import { SpaPage } from './app.po';

describe('spa App', () => {
  let page: SpaPage;

  beforeEach(() => {
    page = new SpaPage();
  });

  it('should display welcome message', () => {
    page.navigateTo();
    expect(page.getParagraphText()).toEqual('Welcome to app!');
  });
});
