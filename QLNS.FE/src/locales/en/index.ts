import { navigation } from './navigation';
import { buttons } from './buttons';
import { labels } from './labels';
import { status } from './status';
import { toast } from './toast';
import { errors } from './errors';
import type { LocaleSchema } from '../vi/index';

export const en: LocaleSchema = {
  ...navigation,
  ...buttons,
  ...labels,
  ...status,
  ...toast,
  ...errors,
};
