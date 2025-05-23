import { UmbDataPathVariantQuery } from '../utils/data-path-variant-query.function.js';
import { UmbAbstractArrayValidationPathTranslator } from './abstract-array-path-translator.controller.js';
import { UmbDeprecation } from '@umbraco-cms/backoffice/utils';
import type { UmbControllerHost } from '@umbraco-cms/backoffice/controller-api';

export class UmbVariantsValidationPathTranslator extends UmbAbstractArrayValidationPathTranslator {
	constructor(host: UmbControllerHost) {
		super(host, '$.variants[', UmbDataPathVariantQuery);

		new UmbDeprecation({
			removeInVersion: '17',
			deprecated: 'UmbVariantsValidationPathTranslator',
			solution: 'UmbVariantsValidationPathTranslator is deprecated.',
		}).warn();
	}

	getDataFromIndex(index: number) {
		if (!this._context) return;
		const data = this._context.getTranslationData();
		return data.variants[index];
	}
}
