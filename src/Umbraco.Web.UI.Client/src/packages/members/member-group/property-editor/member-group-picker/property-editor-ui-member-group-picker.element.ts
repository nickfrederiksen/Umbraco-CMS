import type { UmbInputMemberGroupElement } from '../../components/index.js';
import { html, customElement, property, state } from '@umbraco-cms/backoffice/external/lit';
import { UmbLitElement } from '@umbraco-cms/backoffice/lit-element';
import type { UmbNumberRangeValueType } from '@umbraco-cms/backoffice/models';
import type {
	UmbPropertyEditorConfigCollection,
	UmbPropertyEditorUiElement,
} from '@umbraco-cms/backoffice/property-editor';
import { UmbChangeEvent } from '@umbraco-cms/backoffice/event';

/**
 * @element umb-property-editor-ui-member-group-picker
 */
@customElement('umb-property-editor-ui-member-group-picker')
export class UmbPropertyEditorUIMemberGroupPickerElement extends UmbLitElement implements UmbPropertyEditorUiElement {
	@property()
	public value?: string;

	public set config(config: UmbPropertyEditorConfigCollection | undefined) {
		if (!config) return;

		const minMax = config?.getValueByAlias<UmbNumberRangeValueType>('validationLimit');
		this._min = minMax?.min ?? 0;
		this._max = minMax?.max ?? Infinity;
	}

	/**
	 * Sets the input to readonly mode, meaning value cannot be changed but still able to read and select its content.
	 * @type {boolean}
	 * @attr
	 * @default false
	 */
	@property({ type: Boolean, reflect: true })
	readonly = false;

	@state()
	_min = 0;

	@state()
	_max = Infinity;

	#onChange(event: CustomEvent & { target: UmbInputMemberGroupElement }) {
		this.value = event.target.value;
		this.dispatchEvent(new UmbChangeEvent());
	}

	override render() {
		return html`
			<umb-input-member-group
				.min=${this._min}
				.max=${this._max}
				.value=${this.value}
				@change=${this.#onChange}
				?readonly=${this.readonly}></umb-input-member-group>
		`;
	}
}

export default UmbPropertyEditorUIMemberGroupPickerElement;

declare global {
	interface HTMLElementTagNameMap {
		'umb-property-editor-ui-member-group-picker': UmbPropertyEditorUIMemberGroupPickerElement;
	}
}
