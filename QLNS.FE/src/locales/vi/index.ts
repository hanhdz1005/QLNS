import { navigation } from './navigation';
import { buttons } from './buttons';
import { labels } from './labels';
import { status } from './status';
import { toast } from './toast';
import { errors } from './errors';

export const vi = {
  ...navigation,
  ...buttons,
  ...labels,
  ...status,
  ...toast,
  ...errors,
};

export type LocaleSchema = typeof vi;
