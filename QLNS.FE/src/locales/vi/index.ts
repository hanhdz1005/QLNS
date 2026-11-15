import { navigation } from './navigation';
import { buttons } from './buttons';
import { labels } from './labels';
import { status } from './status';
import { toast } from './toast';
import { errors } from './errors';
import { face } from './face';
import { lifecycle } from './lifecycle';
import { settings } from './settings';

export const vi = {
  ...navigation,
  ...buttons,
  ...labels,
  ...status,
  ...toast,
  ...errors,
  ...face,
  ...lifecycle,
  ...settings,
};

export type LocaleSchema = typeof vi;
