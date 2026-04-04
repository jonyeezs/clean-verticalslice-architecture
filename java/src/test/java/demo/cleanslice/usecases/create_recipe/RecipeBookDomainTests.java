package demo.cleanslice.usecases.create_recipe;

import static org.junit.jupiter.api.Assertions.*;

import java.util.ArrayList;
import java.util.List;
import java.util.function.Function;
import org.junit.jupiter.api.Test;

class RecipeBookDomainTests {

    final String recipeTitle = "test-title";

    @Test
    void givenTheRecipeDoesNotExistItShouldAddItToTheRecipeBook()
        throws RecipeExistsException {
        Function<String, List<Recipe>> getRecipesByTitleStub = title ->
            new ArrayList<Recipe>();
        RecipeBookDomain subject = new RecipeBookDomain(getRecipesByTitleStub);

        subject.addRecipe(new Recipe(recipeTitle, null));

        assertEquals(1, subject.recipes.size());
        assertEquals(subject.recipes.get(0).getTitle(), recipeTitle);
    }

    @Test
    void givenTheRecipeDoesExistItShouldNotAddDuplicateToRecipeBook() {
        Function<String, List<Recipe>> getRecipesByTitleStub = title ->
            List.of(new Recipe(recipeTitle, null));
        RecipeBookDomain subject = new RecipeBookDomain(getRecipesByTitleStub);

        assertThrows(RecipeExistsException.class, () -> {
            subject.addRecipe(new Recipe(recipeTitle, null));
        });
    }
}
