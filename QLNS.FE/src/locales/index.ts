import { ref, computed } from 'vue';
import { vi, type LocaleSchema } from './vi';
import { en } from './en';

export type SupportedLocale = 'vi' | 'en';

const LOCALE_STORAGE_KEY = 'app_locale';
const currentLocale = ref<SupportedLocale>(
  (localStorage.getItem(LOCALE_STORAGE_KEY) as SupportedLocale) || 'vi'
);

const dictionaries: Record<SupportedLocale, LocaleSchema> = {
  vi,
  en,
};

export function useI18n() {
  const locale = computed(() => currentLocale.value);

  const setLocale = (lang: SupportedLocale) => {
    currentLocale.value = lang;
    localStorage.setItem(LOCALE_STORAGE_KEY, lang);
  };

  const toggleLocale = () => {
    setLocale(currentLocale.value === 'vi' ? 'en' : 'vi');
  };

  /**
   * Translate a key (e.g. 'b_delete', 'l_full_name', 't_save_success')
   */
  const t = (key: string, params?: Record<string, any> | string, fallback?: string): string => {
    let fallbackText = fallback;
    let interpolations: Record<string, any> | undefined = undefined;
    if (typeof params === 'string') {
      fallbackText = params;
    } else if (typeof params === 'object' && params !== null) {
      interpolations = params;
    }

    const dict = dictionaries[currentLocale.value] as Record<string, any>;
    let text = dict[key];

    // Fallback to Vietnamese if missing in current locale
    if (text === undefined) {
      const viDict = dictionaries.vi as Record<string, any>;
      text = viDict[key];
    }

    // Support legacy dotted path lookup if key contains '.'
    if (text === undefined && key.includes('.')) {
      const parts = key.split('.');
      let current: any = dict;
      for (const p of parts) {
        if (current && typeof current === 'object' && p in current) {
          current = current[p];
        } else {
          current = undefined;
          break;
        }
      }
      if (typeof current === 'string') {
        text = current;
      }
    }

    let finalStr = typeof text === 'string' ? text : (fallbackText || key);
    if (interpolations) {
      for (const [k, v] of Object.entries(interpolations)) {
        finalStr = finalStr.replace(new RegExp(`{${k}}`, 'g'), String(v));
      }
    }
    return finalStr;
  };

  return {
    locale,
    setLocale,
    toggleLocale,
    t,
  };
}

export { vi, en };
