import { UMB_DOCUMENT_TYPE_FOLDER_ENTITY_TYPE } from '../entity.js';
import type { UmbFolderModel } from '@umbraco-cms/backoffice/tree';
import { DocumentTypeService } from '@umbraco-cms/backoffice/external/backend-api';
import type { UmbControllerHost } from '@umbraco-cms/backoffice/controller-api';
import { tryExecute } from '@umbraco-cms/backoffice/resources';
import { UmbId } from '@umbraco-cms/backoffice/id';
import type { UmbDetailDataSource } from '@umbraco-cms/backoffice/repository';

/**
 * A data source for a Document Type folder that fetches data from the server
 * @class UmbDocumentTypeFolderServerDataSource
 * @implements {RepositoryDetailDataSource}
 */
export class UmbDocumentTypeFolderServerDataSource implements UmbDetailDataSource<UmbFolderModel> {
	#host: UmbControllerHost;

	/**
	 * Creates an instance of UmbDocumentTypeFolderServerDataSource.
	 * @param {UmbControllerHost} host - The controller host for this controller to be appended to
	 * @memberof UmbDocumentTypeFolderServerDataSource
	 */
	constructor(host: UmbControllerHost) {
		this.#host = host;
	}

	/**
	 * Creates a scaffold for a Document Type folder
	 * @param {Partial<UmbFolderModel>} [preset]
	 * @returns {*}
	 * @memberof UmbDocumentTypeFolderServerDataSource
	 */
	async createScaffold(preset?: Partial<UmbFolderModel>) {
		const scaffold: UmbFolderModel = {
			entityType: UMB_DOCUMENT_TYPE_FOLDER_ENTITY_TYPE,
			unique: UmbId.new(),
			name: '',
			...preset,
		};

		return { data: scaffold };
	}

	/**
	 * Fetches a Document Type folder from the server
	 * @param {string} unique
	 * @returns {*}
	 * @memberof UmbDocumentTypeFolderServerDataSource
	 */
	async read(unique: string) {
		if (!unique) throw new Error('Unique is missing');

		const { data, error } = await tryExecute(
			this.#host,
			DocumentTypeService.getDocumentTypeFolderById({
				path: { id: unique },
			}),
		);

		if (data) {
			const mappedData = {
				entityType: UMB_DOCUMENT_TYPE_FOLDER_ENTITY_TYPE,
				unique: data.id,
				name: data.name,
			};

			return { data: mappedData };
		}

		return { error };
	}

	/**
	 * Creates a Document Type folder on the server
	 * @param {UmbCreateFolderModel} model
	 * @returns {*}
	 * @memberof UmbDocumentTypeFolderServerDataSource
	 */
	async create(model: UmbFolderModel, parentUnique: string | null) {
		if (!model) throw new Error('Model is missing');
		if (!model.unique) throw new Error('Unique is missing');
		if (!model.name) throw new Error('Name is missing');

		const body = {
			id: model.unique,
			parent: parentUnique ? { id: parentUnique } : null,
			name: model.name,
		};

		const { error } = await tryExecute(
			this.#host,
			DocumentTypeService.postDocumentTypeFolder({
				body,
			}),
		);

		if (!error) {
			return this.read(model.unique);
		}

		return { error };
	}

	/**
	 * Updates a Document Type folder on the server
	 * @param {UmbFolderModel} model
	 * @returns {*}
	 * @memberof UmbDocumentTypeFolderServerDataSource
	 */
	async update(model: UmbFolderModel) {
		if (!model.unique) throw new Error('Unique is missing');
		if (!model.name) throw new Error('Folder name is missing');

		const { error } = await tryExecute(
			this.#host,
			DocumentTypeService.putDocumentTypeFolderById({
				path: { id: model.unique },
				body: { name: model.name },
			}),
		);

		if (!error) {
			return this.read(model.unique);
		}

		return { error };
	}

	/**
	 * Deletes a Document Type folder on the server
	 * @param {string} unique
	 * @returns {*}
	 * @memberof UmbDocumentTypeServerDataSource
	 */
	async delete(unique: string) {
		if (!unique) throw new Error('Unique is missing');
		return tryExecute(
			this.#host,
			DocumentTypeService.deleteDocumentTypeFolderById({
				path: { id: unique },
			}),
		);
	}
}
